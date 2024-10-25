using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IClusterRbacAssessmentReportService
{
    Task<IList<ClusterRbacAssessmentReportDenormalizedDto>> GetClusterRbacAssessmentReportDenormalizedDtos();
    Task<IList<ClusterRbacAssessmentReportDto>> GetClusterRbacAssessmentReportDtos();
    public Task<IList<ClusterRbacAssessmentReportSummaryDto>> GetClusterRbacAssessmentReportSummaryDtos();
}