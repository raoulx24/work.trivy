using System.Web;
using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class SbomReportDto
{
    public string Uid { get; set; } = Guid.NewGuid().ToString();
    public string? ResourceName { get; init; } = string.Empty;
    public string? ResourceNamespace { get; init; } = string.Empty;
    public string? ResourceKind { get; init; } = string.Empty;
    public string? ResourceContainerName { get; init; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public string ImageTag { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public SbomReportDetailDto[] Details { get; set; } = [];
}

public class SbomReportDetailDto
{
    public Guid BomRef { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Purl { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Guid[] DependsOn { get; set; } = [];
    public string[][] Properties { get; set; } = [];
}

public static class SbomReportCrExtensions
{
    public static SbomReportDto ToSbomReportDto(this SbomReportCr sbomReportCr)
    {
        ComponentsComponent[] allComponents = sbomReportCr.Report?.Components.ComponentsComponents ?? [];
        SanitizeComponents(allComponents);
        Array.Resize(ref allComponents, allComponents.Length + 1);
        allComponents[^1] = new ComponentsComponent
        {
            BomRef = Guid.Empty.ToString(),
            Name = sbomReportCr.Report?.Components.Metadata.Component.Name ?? string.Empty,
            Purl = sbomReportCr.Report?.Components.Metadata.Component.Purl ?? string.Empty,
            Type = sbomReportCr.Report?.Components.Metadata.Component.Type ?? string.Empty,
            Version = sbomReportCr.Report?.Components.Metadata.Component.Version ?? string.Empty,
            Properties = sbomReportCr.Report?.Components.Metadata.Component.Properties ?? []
        };
        Dependency[] alldependencies = sbomReportCr.Report?.Components.Dependencies ?? [];

        IEnumerable<SbomReportDetailDto> details = allComponents.Select(
            component =>
            {
                Guid.TryParse(component.BomRef, out Guid bomRef);

                Dependency refDependency =
                    alldependencies.FirstOrDefault(dep => dep.Ref == bomRef.ToString() || dep.Ref == component.Purl) ??
                    new Dependency();
                Guid[] dependencies = refDependency.DependsOn.Select(
                        depOn =>
                        {
                            string dependsOn =
                                allComponents.FirstOrDefault(dep => dep.BomRef == depOn || dep.Purl == depOn)?.BomRef ??
                                string.Empty;
                            Guid.TryParse(dependsOn, out Guid dependsOnBomRef);

                            return dependsOnBomRef;
                        })
                    .ToArray();
                SbomReportDetailDto detailDto = new()
                {
                    BomRef = bomRef,
                    Name = HttpUtility.HtmlEncode(component.Name),
                    Purl = component.Purl,
                    Version = HttpUtility.HtmlEncode(component.Version),
                    DependsOn = dependencies,
                    Properties = component.Properties.Select(x => new string[] { x.Name, x.Value }).ToArray(),
                };

                return detailDto;
            });

        SbomReportDto result = new()
        {
            Uid = sbomReportCr.Metadata.Uid,
            ResourceName =
                sbomReportCr.Metadata.Labels != null &&
                sbomReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.name", out string? resourceName)
                    ? resourceName
                    : string.Empty,
            ResourceNamespace =
                sbomReportCr.Metadata.Labels != null &&
                sbomReportCr.Metadata.Labels.TryGetValue(
                    "trivy-operator.resource.namespace",
                    out string? resourceNamespace)
                    ? resourceNamespace
                    : string.Empty,
            ResourceKind =
                sbomReportCr.Metadata.Labels != null &&
                sbomReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.kind", out string? resourceKind)
                    ? resourceKind
                    : string.Empty,
            ResourceContainerName =
                sbomReportCr.Metadata.Labels != null &&
                sbomReportCr.Metadata.Labels.TryGetValue(
                    "trivy-operator.container.name",
                    out string? resourceContainerName)
                    ? resourceContainerName
                    : string.Empty,
            ImageName = sbomReportCr.Report?.Artifact?.Repository ?? string.Empty,
            ImageTag = sbomReportCr.Report?.Artifact?.Tag ?? string.Empty,
            Repository = sbomReportCr.Report?.Registry?.Server ?? string.Empty,
            Details = details.ToArray(),
        };

        RemoveSbomDetailDuplicates(result);

        return result;
    }

    private static void SanitizeComponents(ComponentsComponent[] components)
    {
        foreach (ComponentsComponent component in components)
        {
            component.BomRef = SanitizeBomRef(component.BomRef);
        }
    }

    private static void RemoveSbomDetailDuplicates(SbomReportDto sbomReportDto)
    {
        IEnumerable<IGrouping<string, SbomReportDetailDto>> groupedByPurl = sbomReportDto.Details
            .GroupBy(x => string.IsNullOrEmpty(x.Purl) ? Guid.NewGuid().ToString() : x.Purl);

        List<SbomReportDetailDto> uniqueSboms = new List<SbomReportDetailDto>();
        Dictionary<Guid, Guid> guidMapping = new Dictionary<Guid, Guid>();

        foreach (IGrouping<string, SbomReportDetailDto> group in groupedByPurl)
        {
            var retainedSbom = group.First();
            uniqueSboms.Add(retainedSbom);
            
            foreach (SbomReportDetailDto sbom in group) {
                guidMapping[sbom.BomRef] = retainedSbom.BomRef;
            }
        }
        foreach (SbomReportDetailDto sbom in uniqueSboms) { 
            sbom.DependsOn = sbom.DependsOn
                .Select(dep => guidMapping.ContainsKey(dep) ? guidMapping[dep] : dep)
                .Distinct()
                .ToArray();
        }

        sbomReportDto.Details = [.. uniqueSboms];
    }

    private static string SanitizeBomRef(string? bomRef) => string.IsNullOrWhiteSpace(bomRef) || bomRef.Length != 36
        ?
        Guid.NewGuid().ToString()
        : Guid.TryParse(bomRef, out _)
            ? bomRef
            : Guid.NewGuid().ToString();
}
