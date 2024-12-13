using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class SbomReportService(ISbomReportDomainService sbomReportDomainService)
    : ISbomReportService
{
    public async Task<IEnumerable<SbomReportDto>> GetSbomReportDtos()
    {
        IEnumerable<SbomReportDto> result = (await sbomReportDomainService.GetSbomReportCrs())
            .Select(x => x.ToSbomReportDto());

        return result;
    }
}
