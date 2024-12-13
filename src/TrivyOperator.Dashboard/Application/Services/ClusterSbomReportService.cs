using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ClusterSbomReportService(IClusterSbomReportDomainService clusterSbomReportDomainService)
    : IClusterSbomReportService
{
    public async Task<IEnumerable<ClusterSbomReportDto>> GetClusterSbomReportDtos()
    {
        IEnumerable<ClusterSbomReportDto> result = (await clusterSbomReportDomainService.GetClusterSbomReportCrs())
            .Select(x => x.ToClusterSbomReportDto());

        return result;
    }
}
