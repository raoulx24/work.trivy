using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ClusterRbacAssessmentReportService(IConcurrentCache<string, IList<ClusterRbacAssessmentReportCr>> cache, ILogger<ClusterRbacAssessmentReportService> logger) : IClusterRbacAssessmentReportService
{
    public Task<IList<ClusterRbacAssessmentReportDto>> GetClusterRbacAssessmentReportDtos()
    {
        List<ClusterRbacAssessmentReportDto> result = cache
            .SelectMany(kvp => kvp.Value)
            .Select(cr => cr.ToClusterRbacAssessmentReportDto())
            .ToList();

        return Task.FromResult<IList<ClusterRbacAssessmentReportDto>>(result);
    }

    public Task<IList<ClusterRbacAssessmentReportDenormalizedDto>> GetClusterRbacAssessmentReportDenormalizedDtos()
    {
        List<ClusterRbacAssessmentReportDenormalizedDto> result = cache
            .SelectMany(kvp => kvp.Value)
            .SelectMany(cr => cr.ToClusterRbacAssessmentReportDenormalizedDtos())
            .ToList();

        return Task.FromResult<IList<ClusterRbacAssessmentReportDenormalizedDto>>(result);
    }
}
