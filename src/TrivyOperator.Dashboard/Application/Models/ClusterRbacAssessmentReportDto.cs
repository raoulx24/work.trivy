using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ClusterRbacAssessmentReportDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public ClusterRbacAssessmentReportDetailDto[]? Checks { get; init; }

}

public class ClusterRbacAssessmentReportDetailDto
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

public class ClusterRbacAssessmentReportDenormalizedDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
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

public static class ClusterRbacAssessmentReportCrExtensions
{
    public static ClusterRbacAssessmentReportDto ToClusterRbacAssessmentReportDto(this ClusterRbacAssessmentReportCr clusterRbacAssessmentReportCr)
    {
        List<ClusterRbacAssessmentReportDetailDto> clusterRbacAssessmentReportDetailDtos = new();
        foreach(Check check in (clusterRbacAssessmentReportCr?.Report?.Checks ?? []))
        {
            ClusterRbacAssessmentReportDetailDto clusterRbacAssessmentReportDetailDto = new()
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
            clusterRbacAssessmentReportDetailDtos.Add(clusterRbacAssessmentReportDetailDto);
        }
        ClusterRbacAssessmentReportDto clusterRbacAssessmentReportDto = new()
        {
            Uid = new Guid(clusterRbacAssessmentReportCr.Metadata.Uid),
            ResourceName = clusterRbacAssessmentReportCr.Metadata.Annotations.ContainsKey("trivy-operator.resource.name")
                ? clusterRbacAssessmentReportCr.Metadata.Annotations["trivy-operator.resource.name"]
                : string.Empty,
            CriticalCount = clusterRbacAssessmentReportCr.Report?.Summary?.CriticalCount ?? 0,
            HighCount = clusterRbacAssessmentReportCr.Report?.Summary?.HighCount ?? 0,
            MediumCount = clusterRbacAssessmentReportCr.Report?.Summary?.MediumCount ?? 0,
            LowCount = clusterRbacAssessmentReportCr.Report?.Summary?.LowCount ?? 0,
            Checks = clusterRbacAssessmentReportDetailDtos.ToArray(),
        };

        return clusterRbacAssessmentReportDto;
    }

    public static IList<ClusterRbacAssessmentReportDenormalizedDto> ToClusterRbacAssessmentReportDenormalizedDtos(this ClusterRbacAssessmentReportCr clusterRbacAssessmentReportCr)
    {
        List<ClusterRbacAssessmentReportDenormalizedDto> clusterRbacAssessmentReportDetailDtos = new();
        foreach (Check check in clusterRbacAssessmentReportCr.Report.Checks)
        {
            ClusterRbacAssessmentReportDenormalizedDto clusterRbacAssessmentReportDenormalizedDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                Severity = check.Severity,
                Success = check.Success,
                Title = check.Title,

                Uid = new Guid(clusterRbacAssessmentReportCr.Metadata.Uid),
                ResourceName = clusterRbacAssessmentReportCr.Metadata.Annotations.ContainsKey("trivy-operator.resource.name")
                    ? clusterRbacAssessmentReportCr.Metadata.Annotations["trivy-operator.resource.name"]
                    : string.Empty,
                CriticalCount = clusterRbacAssessmentReportCr.Report?.Summary?.CriticalCount ?? 0,
                HighCount = clusterRbacAssessmentReportCr.Report?.Summary?.HighCount ?? 0,
                MediumCount = clusterRbacAssessmentReportCr.Report?.Summary?.MediumCount ?? 0,
                LowCount = clusterRbacAssessmentReportCr.Report?.Summary?.LowCount ?? 0,
            };
            clusterRbacAssessmentReportDetailDtos.Add(clusterRbacAssessmentReportDenormalizedDto);
        }

        return clusterRbacAssessmentReportDetailDtos;
    }
}