using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ClusterRbacAssessmentReportDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public ClusterRbacAssessmentReportDetailDto[] Details { get; init; } = [];
}

public class ClusterRbacAssessmentReportDetailDto
{
    public Guid Uid { get; init; } = Guid.NewGuid();
    public string Category { get; init; } = string.Empty;
    public string CheckId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string[] Messages { get; init; } = [];
    public string Remediation { get; init; } = string.Empty;
    public int SeverityId { get; init; }
    public bool Success { get; init; }
    public string Title { get; init; } = string.Empty;
}

public class ClusterRbacAssessmentReportDenormalizedDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }

    public string Category { get; init; } = string.Empty;
    public string CheckId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string[] Messages { get; init; } = [];
    public string Remediation { get; init; } = string.Empty;
    public int SeverityId { get; init; }
    public bool Success { get; init; }
    public string Title { get; init; } = string.Empty;
}

public class ClusterRbacAssessmentReportSummaryDto
{
    public int SeverityId { get; init; } = 0;
    public int TotalCount { get; init; } = 0;
    public int DistinctCount { get; init; } = 0;
}

public static class ClusterRbacAssessmentReportCrExtensions
{
    public static ClusterRbacAssessmentReportDto ToClusterRbacAssessmentReportDto(
        this ClusterRbacAssessmentReportCr clusterRbacAssessmentReportCr)
    {
        List<ClusterRbacAssessmentReportDetailDto> clusterRbacAssessmentReportDetailDtos = [];
        foreach (Check check in clusterRbacAssessmentReportCr.Report?.Checks ?? [])
        {
            ClusterRbacAssessmentReportDetailDto clusterRbacAssessmentReportDetailDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                SeverityId = (int)check.Severity,
                Success = check.Success,
                Title = check.Title,
            };
            clusterRbacAssessmentReportDetailDtos.Add(clusterRbacAssessmentReportDetailDto);
        }

        ClusterRbacAssessmentReportDto clusterRbacAssessmentReportDto = new()
        {
            Uid = new Guid(clusterRbacAssessmentReportCr.Metadata.Uid ?? string.Empty),
            ResourceName =
                clusterRbacAssessmentReportCr.Metadata.Annotations != null &&
                clusterRbacAssessmentReportCr.Metadata.Annotations.TryGetValue(
                    "trivy-operator.resource.name",
                    out string? resourceName)
                    ? resourceName
                    : string.Empty,
            CriticalCount = clusterRbacAssessmentReportCr.Report?.Summary?.CriticalCount ?? 0,
            HighCount = clusterRbacAssessmentReportCr.Report?.Summary?.HighCount ?? 0,
            MediumCount = clusterRbacAssessmentReportCr.Report?.Summary?.MediumCount ?? 0,
            LowCount = clusterRbacAssessmentReportCr.Report?.Summary?.LowCount ?? 0,
            Details = [.. clusterRbacAssessmentReportDetailDtos],
        };

        return clusterRbacAssessmentReportDto;
    }

    public static IList<ClusterRbacAssessmentReportDenormalizedDto> ToClusterRbacAssessmentReportDenormalizedDtos(
        this ClusterRbacAssessmentReportCr clusterRbacAssessmentReportCr)
    {
        List<ClusterRbacAssessmentReportDenormalizedDto> clusterRbacAssessmentReportDetailDtos = [];
        foreach (Check check in clusterRbacAssessmentReportCr.Report?.Checks ?? [])
        {
            ClusterRbacAssessmentReportDenormalizedDto clusterRbacAssessmentReportDenormalizedDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                SeverityId = (int)check.Severity,
                Success = check.Success,
                Title = check.Title,
                Uid = new Guid(clusterRbacAssessmentReportCr?.Metadata?.Uid ?? string.Empty),
                ResourceName =
                    clusterRbacAssessmentReportCr?.Metadata?.Annotations != null &&
                    clusterRbacAssessmentReportCr.Metadata.Annotations.TryGetValue(
                        "trivy-operator.resource.name",
                        out string? resourceName)
                        ? resourceName
                        : string.Empty,
                CriticalCount = clusterRbacAssessmentReportCr?.Report?.Summary?.CriticalCount ?? 0,
                HighCount = clusterRbacAssessmentReportCr?.Report?.Summary?.HighCount ?? 0,
                MediumCount = clusterRbacAssessmentReportCr?.Report?.Summary?.MediumCount ?? 0,
                LowCount = clusterRbacAssessmentReportCr?.Report?.Summary?.LowCount ?? 0,
            };
            clusterRbacAssessmentReportDetailDtos.Add(clusterRbacAssessmentReportDenormalizedDto);
        }

        return clusterRbacAssessmentReportDetailDtos;
    }
}
