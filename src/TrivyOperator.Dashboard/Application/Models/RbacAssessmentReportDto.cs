using TrivyOperator.Dashboard.Domain.Trivy.RbacAssessmentReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class RbacAssessmentReportDto
{
    public Guid Uid { get; init; } = Guid.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public RbacAssessmentReportDetailDto[] Details { get; init; } = [];
}

public class RbacAssessmentReportDetailDto
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

public class RbacAssessmentReportDenormalizedDto
{
    public Guid Uid { get; init; } = Guid.NewGuid();
    public string ResourceName { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;
    public string CheckId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string[] Messages { get; init; } = [];
    public string Remediation { get; init; } = string.Empty;
    public int SeverityId { get; init; }
    public bool Success { get; init; }
    public string Title { get; init; } = string.Empty;
}

public static class RbacAssessmentReportCrExtensions
{
    public static RbacAssessmentReportDto ToRbacAssessmentReportDto(this RbacAssessmentReportCr rbacAssessmentReportCr)
    {
        List<RbacAssessmentReportDetailDto> rbacAssessmentReportDetailDtos = [];
        foreach (Check check in rbacAssessmentReportCr.Report?.Checks ?? [])
        {
            RbacAssessmentReportDetailDto rbacAssessmentReportDetailDto = new()
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
            rbacAssessmentReportDetailDtos.Add(rbacAssessmentReportDetailDto);
        }

        RbacAssessmentReportDto rbacAssessmentReportDto = new()
        {
            Uid = new Guid(rbacAssessmentReportCr.Metadata.Uid ?? string.Empty),
            ResourceName =
                rbacAssessmentReportCr.Metadata.Annotations != null &&
                rbacAssessmentReportCr.Metadata.Annotations.TryGetValue(
                    "trivy-operator.resource.name",
                    out string? resourceName)
                    ? resourceName
                    : string.Empty,
            CriticalCount = rbacAssessmentReportCr.Report?.Summary?.CriticalCount ?? 0,
            HighCount = rbacAssessmentReportCr.Report?.Summary?.HighCount ?? 0,
            MediumCount = rbacAssessmentReportCr.Report?.Summary?.MediumCount ?? 0,
            LowCount = rbacAssessmentReportCr.Report?.Summary?.LowCount ?? 0,
            Details = [.. rbacAssessmentReportDetailDtos],
        };

        return rbacAssessmentReportDto;
    }

    public static IList<RbacAssessmentReportDenormalizedDto> ToRbacAssessmentReportDenormalizedDtos(
        this RbacAssessmentReportCr rbacAssessmentReportCr)
    {
        List<RbacAssessmentReportDenormalizedDto> rbacAssessmentReportDetailDtos = [];
        string resourceName =
            rbacAssessmentReportCr?.Metadata?.Annotations != null &&
            rbacAssessmentReportCr.Metadata.Annotations.TryGetValue(
                "trivy-operator.resource.name",
                out string? tryResourceName)
                ? tryResourceName
                : string.Empty;

        foreach (Check check in rbacAssessmentReportCr?.Report?.Checks ?? [])
        {
            RbacAssessmentReportDenormalizedDto rbacAssessmentReportDenormalizedDto = new()
            {
                Category = check.Category,
                CheckId = check.CheckId,
                Description = check.Description,
                Messages = check.Messages,
                Remediation = check.Remediation,
                SeverityId = (int)check.Severity,
                Success = check.Success,
                Title = check.Title,
                ResourceName = resourceName,
            };
            rbacAssessmentReportDetailDtos.Add(rbacAssessmentReportDenormalizedDto);
        }

        return rbacAssessmentReportDetailDtos;
    }
}
