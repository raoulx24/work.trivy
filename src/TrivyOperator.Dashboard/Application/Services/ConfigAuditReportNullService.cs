using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ConfigAuditReportNullService : IConfigAuditReportService
{
    public Task<IEnumerable<ConfigAuditReportDto>> GetConfigAuditReportDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null) => Task.FromResult<IEnumerable<ConfigAuditReportDto>>([]);

    public Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(
        string? namespaceName = null) => Task.FromResult<IList<ConfigAuditReportDenormalizedDto>>([]);

    public Task<IEnumerable<string>> GetActiveNamespaces() => Task.FromResult<IEnumerable<string>>([]);

    public Task<IEnumerable<ConfigAuditReportSummaryDto>> GetConfigAuditReportSummaryDtos() =>
        Task.FromResult<IEnumerable<ConfigAuditReportSummaryDto>>([]);
}
