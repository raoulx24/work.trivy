using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class RbacAssessmentReportNullService : IRbacAssessmentReportService
{
    public Task<IEnumerable<string>> GetActiveNamespaces() =>
        Task.FromResult<IEnumerable<string>>([]);

    public Task<IList<RbacAssessmentReportDenormalizedDto>> GetRbacAssessmentReportDenormalizedDtos(
        string? namespaceName = null) =>
        Task.FromResult<IList<RbacAssessmentReportDenormalizedDto>>([]);

    public Task<IEnumerable<RbacAssessmentReportDto>> GetRbacAssessmentReportDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null) =>
        Task.FromResult<IEnumerable<RbacAssessmentReportDto>>([]);
}
