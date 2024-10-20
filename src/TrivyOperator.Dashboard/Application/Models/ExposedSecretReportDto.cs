using System.Runtime.CompilerServices;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ExposedSecretReportDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public string ResourceNamespace { get; init; } = string.Empty;
    public string ResourceKind { get; init; } = string.Empty;
    public string ResourceContainerName { get; init; } = string.Empty;
    public string ImageName { get; init; } = string.Empty;
    public string ImageTag { get; init; } = string.Empty;
    public string ImageDigest {  get; init; } = string.Empty;
    public string ImageRepository { get; init; } = string.Empty;
    public long CriticalCount { get; init; } = 0;
    public long HighCount { get; init; } = 0;
    public long MediumCount { get; init; } = 0;
    public long LowCount { get; init; } = 0;
    public ExposedSecretReportDetailDto[] Details { get; set; } = [];
}

public class ExposedSecretReportImageDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceNamespace { get; init; } = string.Empty;
    public string ImageName { get; init; } = String.Empty;
    public string ImageTag { get; init; } = string.Empty;
    public string ImageRepository { get; init; } = string.Empty;
    public List<ExposedSecretReportImageResourceDto> Resources { get; init; } = [];
    public long CriticalCount { get; init; } = 0;
    public long HighCount { get; init; } = 0;
    public long MediumCount { get; init; } = 0;
    public long LowCount { get; init; } = 0;
    public ExposedSecretReportDetailDto[] Details { get; set; } = [];
}

public class ExposedSecretReportImageResourceDto
{
    public string Name { get; init; } = string.Empty;
    public string Kind { get; init; } = string.Empty;
    public string ContainerName { get; init; } = string.Empty;
}

public class ExposedSecretReportDetailDto
{
    public string Category { get; init; } = string.Empty;
    public string Match { get; init; } = string.Empty;
    public string RuleId { get; init; } = string.Empty;
    public int SeverityId { get; init; }
    public string Target { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
}

public class ExposedSecretReportDenormalizedDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public string ResourceNamespace { get; init; } = string.Empty;
    public string ResourceKind { get; init; } = string.Empty;
    public string ResourceContainerName { get; init; } = string.Empty;
    public string ImageName { get; init; } = string.Empty;
    public string ImageTag { get; init; } = string.Empty;
    public string ImageDigest {  get; init; } = string.Empty;
    public string ImageRepository { get; init; } = string.Empty;
    public long CriticalCount { get; init; } = 0;
    public long HighCount { get; init; } = 0;
    public long MediumCount { get; init; } = 0;
    public long LowCount { get; init; } = 0;

    public string Category { get; init; } = string.Empty;
    public string Match { get; init; } = string.Empty;
    public string RuleId { get; init; } = string.Empty;
    public int SeverityId { get; init; }
    public string Target { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
}

public static class ExposedSecretReportCrExtensions
{
    public static ExposedSecretReportDto ToExposedSecretReportDto(this ExposedSecretReportCr exposedSecretReportCr)
    {
        List<ExposedSecretReportDetailDto> exposedSecretReportDetailDtos = [];
        foreach (Secret secret in (exposedSecretReportCr?.Report?.Secrets ?? []))
        {
            ExposedSecretReportDetailDto exposedSecretReportDetailDto = new()
            {
                Category = secret.Category,
                Match = secret.Match,
                RuleId = secret.RuleId,
                SeverityId = (int)secret.Severity,
                Target = secret.Target,
                Title = secret.Title,
            };
            exposedSecretReportDetailDtos.Add(exposedSecretReportDetailDto);
        }
        ExposedSecretReportDto exposedSecretReportDto = new()
        {
            Uid = new Guid(exposedSecretReportCr?.Metadata?.Uid ?? string.Empty),
            ResourceName = exposedSecretReportCr?.Metadata?.Labels != null 
              && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.name", out string? resourceName) ? resourceName : string.Empty,
            ResourceNamespace = exposedSecretReportCr?.Metadata?.Labels != null
              && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.namespace", out string? resourceNamespace) ? resourceNamespace : string.Empty,
            ResourceKind = exposedSecretReportCr?.Metadata?.Labels != null
              && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.kind", out string? resourceKind) ? resourceKind : string.Empty,
            ResourceContainerName = exposedSecretReportCr?.Metadata?.Labels != null
              && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.container.name", out string? resourceContainerName) ? resourceContainerName : string.Empty,
            ImageName = exposedSecretReportCr?.Report?.Artifact?.Repository ?? string.Empty,
            ImageTag = exposedSecretReportCr?.Report?.Artifact?.Tag ?? string.Empty,
            ImageDigest = exposedSecretReportCr?.Report?.Artifact?.Digest ?? string.Empty,
            ImageRepository = exposedSecretReportCr?.Report?.Registry?.Server ?? string.Empty,
            CriticalCount = exposedSecretReportCr?.Report?.Summary?.CriticalCount ?? 0,
            HighCount = exposedSecretReportCr?.Report?.Summary?.HighCount ?? 0,
            MediumCount = exposedSecretReportCr?.Report?.Summary?.MediumCount ?? 0,
            LowCount = exposedSecretReportCr?.Report?.Summary?.LowCount ?? 0,
            Details = [.. exposedSecretReportDetailDtos],
        };

        return exposedSecretReportDto;
    }

    public static ExposedSecretReportImageDto ToExposedSecretReportImageDto(
        this IGrouping<string?, ExposedSecretReportCr> groupedExposedSecretReportCR, IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities = excludedSeverities ?? [];
        List<ExposedSecretReportImageResourceDto> eseirDtos = [];
        foreach (ExposedSecretReportCr vr in groupedExposedSecretReportCR)
        {
            ExposedSecretReportImageResourceDto eseirDto = new()
            {
                Name = vr.Metadata.Labels.TryGetValue("trivy-operator.resource.name", out string? name) ? name : string.Empty,
                ContainerName = vr.Metadata.Labels.TryGetValue("trivy-operator.container.name", out string? containerName) ? containerName : string.Empty,
                Kind = vr.Metadata.Labels.TryGetValue("trivy-operator.resource.kind", out string? kind) ? kind : string.Empty,
            };
            eseirDtos.Add(eseirDto);
        }
        ExposedSecretReportCr? latestExposedSecretReportCr = groupedExposedSecretReportCR?.OrderByDescending(x => x.Report?.UpdateTimestamp).FirstOrDefault();
        List<ExposedSecretReportDetailDto> exposedSecretReportDetailDtos = [];
        foreach (Secret? secret in latestExposedSecretReportCr?.Report?.Secrets ?? [])
        {
            if (!excludedSeverities.Contains((int)secret.Severity))
            {
                ExposedSecretReportDetailDto exposedSecretReportDetailDto = new()
                {
                    Category = secret.Category,
                    Match = secret.Match,
                    RuleId = secret.RuleId,
                    SeverityId = (int)secret.Severity,
                    Target = secret.Target,
                    Title = secret.Title,
                };
                exposedSecretReportDetailDtos.Add(exposedSecretReportDetailDto);
            }
        }
        ExposedSecretReportImageDto exposedSecretReportImageDto = new()
        {
            Uid = new Guid(latestExposedSecretReportCr?.Metadata?.Uid ?? string.Empty),
            ResourceNamespace = latestExposedSecretReportCr?.Metadata?.Labels != null
                && latestExposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.namespace", out string? resourceNamespace) ? resourceNamespace : string.Empty,
            Resources = eseirDtos,
            ImageName = latestExposedSecretReportCr?.Report?.Artifact?.Repository ?? string.Empty,
            ImageTag = latestExposedSecretReportCr?.Report?.Artifact?.Tag ?? string.Empty,
            ImageRepository = latestExposedSecretReportCr?.Report?.Registry?.Server ?? string.Empty,
            CriticalCount = latestExposedSecretReportCr?.Report?.Summary?.CriticalCount ?? 0,
            HighCount = latestExposedSecretReportCr?.Report?.Summary?.HighCount ?? 0,
            MediumCount = latestExposedSecretReportCr?.Report?.Summary?.MediumCount ?? 0,
            LowCount = latestExposedSecretReportCr?.Report?.Summary?.LowCount ?? 0,
            Details = [.. exposedSecretReportDetailDtos],
        };

        return exposedSecretReportImageDto;
    }

    public static IList<ExposedSecretReportDenormalizedDto> ToExposedSecretReportDenormalizedDtos(this ExposedSecretReportCr exposedSecretReportCr)
    {
        List<ExposedSecretReportDenormalizedDto> exposedSecretReportDenormalizedDtos = [];
        foreach (Secret secret in exposedSecretReportCr?.Report?.Secrets ?? [])
        {
            ExposedSecretReportDenormalizedDto exposedSecretReportDenormalizedDto = new()
            {
                Category = secret.Category,
                Match = secret.Match,
                RuleId = secret.RuleId,
                SeverityId = (int)secret.Severity,
                Target = secret.Target,
                Title = secret.Title,

                Uid = new Guid(exposedSecretReportCr?.Metadata?.Uid ?? string.Empty),
                ResourceName = exposedSecretReportCr?.Metadata?.Labels != null 
                    && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.name", out string? resourceName) ? resourceName : string.Empty,
                ResourceNamespace = exposedSecretReportCr?.Metadata?.Labels != null
                    && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.namespace", out string? resourceNamespace) ? resourceNamespace : string.Empty,
                ResourceKind = exposedSecretReportCr?.Metadata?.Labels != null
                    && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.resource.kind", out string? resourceKind) ? resourceKind : string.Empty,
                ResourceContainerName = exposedSecretReportCr?.Metadata?.Labels != null
                    && exposedSecretReportCr.Metadata.Labels.TryGetValue("trivy-operator.container.name", out string? resourceContainerName) ? resourceContainerName : string.Empty,
                ImageName = exposedSecretReportCr?.Report?.Artifact?.Repository ?? string.Empty,
                ImageTag = exposedSecretReportCr?.Report?.Artifact?.Tag ?? string.Empty,
                ImageDigest = exposedSecretReportCr?.Report?.Artifact?.Digest ?? string.Empty,
                ImageRepository = exposedSecretReportCr?.Report?.Registry?.Server ?? string.Empty,
                CriticalCount = exposedSecretReportCr?.Report?.Summary?.CriticalCount ?? 0,
                HighCount = exposedSecretReportCr?.Report?.Summary?.HighCount ?? 0,
                MediumCount = exposedSecretReportCr?.Report?.Summary?.MediumCount ?? 0,
                LowCount = exposedSecretReportCr?.Report?.Summary?.LowCount ?? 0,
            };
            exposedSecretReportDenormalizedDtos.Add(exposedSecretReportDenormalizedDto);
        }

        return exposedSecretReportDenormalizedDtos;
    }
}