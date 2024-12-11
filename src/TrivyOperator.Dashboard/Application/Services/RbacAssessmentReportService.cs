using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.RbacAssessmentReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class RbacAssessmentReportService(IConcurrentCache<string, IList<RbacAssessmentReportCr>> cache)
    : IRbacAssessmentReportService
{
    public Task<IEnumerable<RbacAssessmentReportDto>> GetRbacAssessmentReportDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities ??= [];
        int[] excludedSeveritiesArray = excludedSeverities.ToArray();
        int[] includedSeverities = ((int[])Enum.GetValues(typeof(TrivySeverity))).ToList()
            .Except(excludedSeveritiesArray)
            .ToArray();

        IEnumerable<RbacAssessmentReportDto> dtos = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(
                kvp => kvp.Value.Select(cr => cr.ToRbacAssessmentReportDto())
                    .Select(
                        dto =>
                        {
                            dto.Details = dto.Details.Join(
                                    includedSeverities,
                                    vulnerability => vulnerability.SeverityId,
                                    id => id,
                                    (vulnerability, _) => vulnerability)
                                .ToArray();
                            return dto;
                        })
                    .Where(dto => excludedSeveritiesArray.Length == 0 || dto.Details.Length != 0));

        return Task.FromResult(dtos);
    }

    public Task<IList<RbacAssessmentReportDenormalizedDto>> GetRbacAssessmentReportDenormalizedDtos(
        string? namespaceName = null)
    {
        List<RbacAssessmentReportDenormalizedDto> result = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value)
            .SelectMany(cr => cr.ToRbacAssessmentReportDenormalizedDtos())
            .ToList();

        return Task.FromResult<IList<RbacAssessmentReportDenormalizedDto>>(result);
    }

    public Task<IEnumerable<string>> GetActiveNamespaces() =>
        Task.FromResult(cache.Where(x => x.Value.Any()).Select(x => x.Key));
}
