using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;

public interface ISbomReportDomainService
{
    Task<IList<SbomReportCr>> GetSbomReportCrs();
    Task<IList<SbomReportCr>> GetSbomReportCrs(string resourceNamespace);
    Task<SbomReportCr> GetSbomReportCr(string resourceName, string resourceNamespace);
}
