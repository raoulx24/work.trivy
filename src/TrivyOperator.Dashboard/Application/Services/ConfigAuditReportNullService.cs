using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ConfigAuditReportNullService(ILogger<ConfigAuditReportNullService> logger) : IConfigAuditReportService
{
    public Task<IEnumerable<ConfigAuditReportDto>> GetConfigAuditReportDtos(string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    { return Task.FromResult<IEnumerable<ConfigAuditReportDto>>([]); }

    public Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(string? namespaceName = null)
    { return Task.FromResult<IList<ConfigAuditReportDenormalizedDto>>([]); }

    public Task<IEnumerable<string>> GetActiveNamespaces()
    { return Task.FromResult<IEnumerable<string>>([]); }
}
