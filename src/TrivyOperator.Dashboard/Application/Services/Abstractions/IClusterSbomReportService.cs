using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IClusterSbomReportService
{
    Task<IEnumerable<ClusterSbomReportDto>> GetClusterSbomReportDtos();
}