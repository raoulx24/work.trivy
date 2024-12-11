using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IClusterComplianceReportService
{
    Task<IEnumerable<ClusterComplianceReportDenormalizedDto>> GetClusterComplianceReportDenormalizedDtos();
    Task<IEnumerable<ClusterComplianceReportDto>> GetClusterComplianceReportDtos();
}
