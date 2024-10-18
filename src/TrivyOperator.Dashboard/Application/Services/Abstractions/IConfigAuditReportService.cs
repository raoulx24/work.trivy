using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IConfigAuditReportService
{
    Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(string? namespaceName = null);
    Task<IList<ConfigAuditReportDto>> GetConfigAuditReportDtos(string? namespaceName = null);
}