using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ClusterRbacAssessmentReportNullService : IClusterRbacAssessmentReportService
{
    public Task<IList<ClusterRbacAssessmentReportDto>> GetClusterRbacAssessmentReportDtos() =>
        Task.FromResult<IList<ClusterRbacAssessmentReportDto>>([]);

    public Task<IList<ClusterRbacAssessmentReportDenormalizedDto>> GetClusterRbacAssessmentReportDenormalizedDtos() =>
        Task.FromResult<IList<ClusterRbacAssessmentReportDenormalizedDto>>([]);

    public Task<IList<ClusterRbacAssessmentReportSummaryDto>> GetClusterRbacAssessmentReportSummaryDtos() =>
        Task.FromResult<IList<ClusterRbacAssessmentReportSummaryDto>>([]);
}
