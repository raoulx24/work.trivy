using TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;
public interface IClusterSbomReportDomainService
{
    Task<IList<ClusterSbomReportCr>> GetClusterSbomReportCrs();
}