using System.Runtime.CompilerServices;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ExposedSecretReportDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public string? ResourceContainerName { get; init; }
    public string? ImageName { get; init; }
    public string? ImageTag { get; init; }
    public string? ImageRepository { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public ExposedSecretReportDetailDto[]? Secrets { get; init; }
}

public class ExposedSecretReportDetailDto
{
    public string? Category { get; init; }
    public string? Match { get; init; }
    public string? RuleId { get; init; }
    public string? Severity { get; init; }
    public string? Target { get; init; }
    public string? Title { get; init; }
}

public class ExposedSecretReportDenormalizedDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public string? ResourceContainerName { get; init; }
    public string? ImageName { get; init; }
    public string? ImageTag { get; init; }
    public string? ImageRepository { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }

    public string? Category { get; init; }
    public string? Match { get; init; }
    public string? RuleId { get; init; }
    public string? Severity { get; init; }
    public string? Target { get; init; }
    public string? Title { get; init; }
}

public static class ExposedSecretReportCrExtensions
{
    public static ExposedSecretReportDto ToExposedSecretReportDto(this ExposedSecretReportCr exposedSecretReportCr)
    {
        List<ExposedSecretReportDetailDto> exposedSecretReportDetailDtos = new();
        foreach (Secret secret in (exposedSecretReportCr?.Report?.Secrets ?? []))
        {
            ExposedSecretReportDetailDto exposedSecretReportDetailDto = new ExposedSecretReportDetailDto()
            {
                Category = secret.Category,
                Match = secret.Match,
                RuleId = secret.RuleId,
                Severity = secret.Severity,
                Target = secret.Target,
                Title = secret.Title,
            };
            exposedSecretReportDetailDtos.Add(exposedSecretReportDetailDto);
        }
        ExposedSecretReportDto exposedSecretReportDto = new()
        {
            Uid = new Guid(exposedSecretReportCr.Metadata.Uid),
            ResourceName = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
            ResourceNamespace = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
            ResourceKind = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
            ResourceContainerName = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.container.name")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.container.name"]
                : string.Empty,
            ImageName = exposedSecretReportCr.Report.Artifact.Repository,
            ImageTag = exposedSecretReportCr.Report.Artifact.Tag,
            ImageRepository = exposedSecretReportCr.Report.Registry.Server,
            CriticalCount = exposedSecretReportCr.Report?.Summary?.CriticalCount ?? 0,
            HighCount = exposedSecretReportCr.Report?.Summary?.HighCount ?? 0,
            MediumCount = exposedSecretReportCr.Report?.Summary?.MediumCount ?? 0,
            LowCount = exposedSecretReportCr.Report?.Summary?.LowCount ?? 0,
            Secrets = exposedSecretReportDetailDtos.ToArray(),
        };

        return exposedSecretReportDto;
    }

    public static IList<ExposedSecretReportDenormalizedDto> ToExposedSecretReportDenormalizedDtos(this ExposedSecretReportCr exposedSecretReportCr)
    {
        List<ExposedSecretReportDenormalizedDto> exposedSecretReportDenormalizedDtos = new();
        foreach (Secret secret in exposedSecretReportCr.Report.Secrets)
        {
            ExposedSecretReportDenormalizedDto exposedSecretReportDenormalizedDto = new()
            {
                Category = secret.Category,
                Match = secret.Match,
                RuleId = secret.RuleId,
                Severity = secret.Severity,
                Target = secret.Target,
                Title = secret.Title,

                Uid = new Guid(exposedSecretReportCr.Metadata.Uid),
                ResourceName = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
                ResourceNamespace = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
                ResourceKind = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
                ResourceContainerName = exposedSecretReportCr.Metadata.Labels.ContainsKey("trivy-operator.container.name")
                ? exposedSecretReportCr.Metadata.Labels["trivy-operator.container.name"]
                : string.Empty,
                ImageName = exposedSecretReportCr.Report.Artifact.Repository,
                ImageTag = exposedSecretReportCr.Report.Artifact.Tag,
                ImageRepository = exposedSecretReportCr.Report.Registry.Server,
                CriticalCount = exposedSecretReportCr.Report?.Summary?.CriticalCount ?? 0,
                HighCount = exposedSecretReportCr.Report?.Summary?.HighCount ?? 0,
                MediumCount = exposedSecretReportCr.Report?.Summary?.MediumCount ?? 0,
                LowCount = exposedSecretReportCr.Report?.Summary?.LowCount ?? 0,
            };
            exposedSecretReportDenormalizedDtos.Add(exposedSecretReportDenormalizedDto);
        }

        return exposedSecretReportDenormalizedDtos;
    }
}