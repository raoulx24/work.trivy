using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
namespace TrivyOperator.Dashboard.Application.Services;

public class ClusterComplianceReportNullService() : IClusterComplianceReportService
{
    public Task<IEnumerable<ClusterComplianceReportDto>> GetClusterComplianceReportDtos() =>
        Task.FromResult<IEnumerable<ClusterComplianceReportDto>>([]);

    public Task<IEnumerable<ClusterComplianceReportDenormalizedDto>> GetClusterComplianceReportDenormalizedDtos() =>
        Task.FromResult<IEnumerable<ClusterComplianceReportDenormalizedDto>>([]);
    
}
