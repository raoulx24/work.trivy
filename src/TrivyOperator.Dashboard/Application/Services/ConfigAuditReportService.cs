using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ConfigAuditReportService(IConcurrentCache<string, IList<ConfigAuditReportCr>> cache, ILogger<ConfigAuditReportService> logger) : IConfigAuditReportService
{
    public Task<IEnumerable<ConfigAuditReportDto>> GetConfigAuditReportDtos(string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities ??= [];
        int[] incudedSeverities = ((int[])Enum.GetValues(typeof(TrivySeverity))).ToList().Except(excludedSeverities).ToArray();

        IEnumerable<ConfigAuditReportDto> dtos = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value
                .Select(cr => cr.ToConfigAuditReportDto())
                .Select(dto =>
                {
                    dto.Details = dto.Details
                        .Join(incudedSeverities, vulnerability => vulnerability.SeverityId, id => id, (vulnerability, id) => vulnerability)
                        .ToArray();
                    return dto;
                })
                .Where(dto => dto.Details.Length != 0)
            );

        return Task.FromResult<IEnumerable<ConfigAuditReportDto>>(dtos);
    }

    public Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(string? namespaceName = null)
    {
        List<ConfigAuditReportDenormalizedDto> result = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value)
            .SelectMany(car => car.ToConfigAuditReportDetailDenormalizedDtos())
            .ToList();

        return Task.FromResult<IList<ConfigAuditReportDenormalizedDto>>(result);
    }

    public Task<IEnumerable<string>> GetActiveNamespaces()
    {
        return Task.FromResult(cache.Where(x => x.Value.Any()).Select(x => x.Key));
    }
}
