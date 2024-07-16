using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ClusterRbacAssessmentReportDto
{
    public Guid Uid { get; set; }
    public string? ResourceName { get; set; }
    public long CriticalCount { get; set; }
    public long HighCount { get; set; }
    public long MediumCount { get; set; }
    public long LowCount { get; set; }
    public ClusterRbacAssessmentReportDetailDto[]? Checks { get; set; }

}

public class ClusterRbacAssessmentReportDetailDto
{
    public string? Category { get; set; }
    public string? CheckId { get; set; }
    public string? Description { get; set; }
    public string[]? Messages { get; set; }
    public string? Remediation { get; set; }
    public string? Severity { get; set; }
    public bool Success { get; set; }
    public string? Title { get; set; }
}

public class ClusterRbacAssessmentReportDenormalizedDto
{
    public Guid Uid { get; set; }
    public string? ResourceName { get; set; }
    public long CriticalCount { get; set; }
    public long HighCount { get; set; }
    public long MediumCount { get; set; }
    public long LowCount { get; set; }

    public string? Category { get; set; }
    public string? CheckId { get; set; }
    public string? Description { get; set; }
    public string[]? Messages { get; set; }
    public string? Remediation { get; set; }
    public string? Severity { get; set; }
    public bool Success { get; set; }
    public string? Title { get; set; }
}

public static class ClusterRbacAssessmentReportCRExtensions
{
    public static ClusterRbacAssessmentReportDto ToClusterRbacAssessmentReportDto(this ClusterRbacAssessmentReportCR clusterRbacAssessmentReportCR)
    {
        List<ClusterRbacAssessmentReportDetailDto> clusterRbacAssessmentReportDetailDtos = new();
        foreach(Check check in clusterRbacAssessmentReportCR.Report.Checks)
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
            Uid = new Guid(clusterRbacAssessmentReportCR.Metadata.Uid),
            ResourceName = clusterRbacAssessmentReportCR.Metadata.Annotations.ContainsKey("trivy-operator.resource.name")
                ? clusterRbacAssessmentReportCR.Metadata.Annotations["trivy-operator.resource.name"]
                : string.Empty,
            CriticalCount = clusterRbacAssessmentReportCR.Report.Summary.CriticalCount,
            HighCount = clusterRbacAssessmentReportCR.Report.Summary.HighCount,
            MediumCount = clusterRbacAssessmentReportCR.Report.Summary.MediumCount,
            LowCount = clusterRbacAssessmentReportCR.Report.Summary.LowCount,
            Checks = clusterRbacAssessmentReportDetailDtos.ToArray(),
        };

        return clusterRbacAssessmentReportDto;
    }

    public static IList<ClusterRbacAssessmentReportDenormalizedDto> ToClusterRbacAssessmentReportDenormalizedDto(this ClusterRbacAssessmentReportCR clusterRbacAssessmentReportCR)
    {
        List<ClusterRbacAssessmentReportDenormalizedDto> clusterRbacAssessmentReportDetailDtos = new();
        foreach (Check check in clusterRbacAssessmentReportCR.Report.Checks)
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

                Uid = new Guid(clusterRbacAssessmentReportCR.Metadata.Uid),
                ResourceName = clusterRbacAssessmentReportCR.Metadata.Annotations.ContainsKey("trivy-operator.resource.name")
                    ? clusterRbacAssessmentReportCR.Metadata.Annotations["trivy-operator.resource.name"]
                    : string.Empty,
                CriticalCount = clusterRbacAssessmentReportCR.Report.Summary.CriticalCount,
                HighCount = clusterRbacAssessmentReportCR.Report.Summary.HighCount,
                MediumCount = clusterRbacAssessmentReportCR.Report.Summary.MediumCount,
                LowCount = clusterRbacAssessmentReportCR.Report.Summary.LowCount,
            };
            clusterRbacAssessmentReportDetailDtos.Add(clusterRbacAssessmentReportDenormalizedDto);
        }

        return clusterRbacAssessmentReportDetailDtos;
    }
}