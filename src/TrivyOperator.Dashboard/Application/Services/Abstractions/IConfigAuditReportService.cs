using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IConfigAuditReportService
{
    Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(string? namespaceName = null);
    Task<IEnumerable<ConfigAuditReportDto>> GetConfigAuditReportDtos(string? namespaceName = null, IEnumerable<int>? excludedSeverities = null);
    Task<IEnumerable<string>> GetActiveNamespaces();
}