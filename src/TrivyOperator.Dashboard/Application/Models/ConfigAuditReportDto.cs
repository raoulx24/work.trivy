using k8s.Models;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ConfigAuditReportDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public ConfigAuditReportDetailDto[]? Checks { get; init; }
}

public class ConfigAuditReportDetailDto
{
    public string? Category { get; init; }
    public string? CheckId { get; init; }
    public string? Description { get; init; }
    public string[]? Messages { get; init; }
    public string? Remediation { get; init; }
    public string? Severity { get; init; }
    public bool Success { get; init; }
    public string? Title { get; init; }
}

public class ConfigAuditReportDenormalizedDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    
    public string? Category { get; init; }
    public string? CheckId { get; init; }
    public string? Description { get; init; }
    public string[]? Messages { get; init; }
    public string? Remediation { get; init; }
    public string? Severity { get; init; }
    public bool Success { get; init; }
    public string? Title { get; init; }
}

public static class ConfigAuditReportCrExtensions
{
    public static ConfigAuditReportDto ToConfigAuditReportDto(this ConfigAuditReportCr configAuditReportCr)
    {
        List<ConfigAuditReportDetailDto> configAuditReportDetailDtos = new();
        foreach (Check check in (configAuditReportCr?.Report?.Checks ?? []))
        {
            ConfigAuditReportDetailDto configAuditReportDetailDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                Severity = check.Severity,
                Success = check.Success,
                Title = check.Title,
            };
            configAuditReportDetailDtos.Add(configAuditReportDetailDto);
        }
        ConfigAuditReportDto configAuditReportDto = new()
        {
            Uid = new Guid(configAuditReportCr.Metadata.Uid),
            ResourceName = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
            ResourceNamespace = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
            ResourceKind = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
            CriticalCount = configAuditReportCr.Report?.Summary?.CriticalCount ?? 0,
            HighCount = configAuditReportCr.Report?.Summary?.HighCount ?? 0,
            MediumCount = configAuditReportCr.Report?.Summary?.MediumCount ?? 0,
            LowCount = configAuditReportCr.Report?.Summary?.LowCount ?? 0,
            Checks = configAuditReportDetailDtos.ToArray(),
        };

        return configAuditReportDto;
    }

    public static IList<ConfigAuditReportDenormalizedDto> ToConfigAuditReportDetailDenormalizedDtos(this ConfigAuditReportCr configAuditReportCr)
    {
        if (configAuditReportCr is null) throw new ArgumentNullException(nameof(configAuditReportCr));
        List<ConfigAuditReportDenormalizedDto> configAuditReportDenormalizedDtos = new();
        foreach (Check check in configAuditReportCr.Report.Checks)
        {
            ConfigAuditReportDenormalizedDto configAuditReportDenormalizedDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                Severity = check.Severity,
                Success = check.Success,
                Title = check.Title,

                Uid = new Guid(configAuditReportCr.Metadata.Uid),
                ResourceName = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
                ResourceNamespace = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
                ResourceKind = configAuditReportCr.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? configAuditReportCr.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
                CriticalCount = configAuditReportCr.Report?.Summary?.CriticalCount ?? 0,
                HighCount = configAuditReportCr.Report?.Summary?.HighCount ?? 0,
                MediumCount = configAuditReportCr.Report?.Summary?.MediumCount ?? 0,
                LowCount = configAuditReportCr.Report?.Summary?.LowCount ?? 0,
            };
            configAuditReportDenormalizedDtos.Add(configAuditReportDenormalizedDto);
        }
        
        return configAuditReportDenormalizedDtos;
    }
}
