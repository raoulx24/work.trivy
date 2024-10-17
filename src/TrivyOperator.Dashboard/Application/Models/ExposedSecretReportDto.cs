using System.Runtime.CompilerServices;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

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
    public string ImageRepository { get; init; } = string.Empty;
    public long CriticalCount { get; init; } = 0;
    public long HighCount { get; init; } = 0;
    public long MediumCount { get; init; } = 0;
    public long LowCount { get; init; } = 0;
    public ExposedSecretReportDetailDto[] Details { get; init; } = [];
}

public class ExposedSecretReportDetailDto
{
    public string Category { get; init; } = string.Empty;
    public string Match { get; init; } = string.Empty;
    public string RuleId { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
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
    public string ImageRepository { get; init; } = string.Empty;
    public long CriticalCount { get; init; } = 0;
    public long HighCount { get; init; } = 0;
    public long MediumCount { get; init; } = 0;
    public long LowCount { get; init; } = 0;

    public string Category { get; init; } = string.Empty;
    public string Match { get; init; } = string.Empty;
    public string RuleId { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
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
                Severity = secret.Severity,
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
            ImageRepository = exposedSecretReportCr?.Report?.Registry?.Server ?? string.Empty,
            CriticalCount = exposedSecretReportCr?.Report?.Summary?.CriticalCount ?? 0,
            HighCount = exposedSecretReportCr?.Report?.Summary?.HighCount ?? 0,
            MediumCount = exposedSecretReportCr?.Report?.Summary?.MediumCount ?? 0,
            LowCount = exposedSecretReportCr?.Report?.Summary?.LowCount ?? 0,
            Details = [.. exposedSecretReportDetailDtos],
        };

        return exposedSecretReportDto;
    }

    public static IList<ExposedSecretReportDenormalizedDto> ToExposedSecretReportDenormalizedDtos(this ExposedSecretReportCr exposedSecretReportCr)
    {
        List<ExposedSecretReportDenormalizedDto> exposedSecretReportDenormalizedDtos = new();
        foreach (Secret secret in exposedSecretReportCr?.Report?.Secrets ?? [])
        {
            ExposedSecretReportDenormalizedDto exposedSecretReportDenormalizedDto = new()
            {
                Category = secret.Category,
                Match = secret.Match,
                RuleId = secret.RuleId,
                Severity = secret.Severity,
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