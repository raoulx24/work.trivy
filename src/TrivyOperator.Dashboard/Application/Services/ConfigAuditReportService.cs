using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ConfigAuditReportService(IConcurrentCache<string, IList<ConfigAuditReportCr>> cache, ILogger<ConfigAuditReportService> logger) : IConfigAuditReportService
{
    public Task<IList<ConfigAuditReportDto>> GetConfigAuditReportDtos(string? namespaceName = null)
    {
        List<ConfigAuditReportDto> result = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value)
            .Select(car => car.ToConfigAuditReportDto())
            .ToList();

        return Task.FromResult<IList<ConfigAuditReportDto>>(result);
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
