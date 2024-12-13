
using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;

public interface ISbomReportDomainService
{
    Task<IList<SbomReportCr>> GetSbomReportCrs();
}
