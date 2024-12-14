using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface ISbomReportService
{
    Task<IEnumerable<SbomReportDto>> GetSbomReportDtos(string? namespaceName = null);
    Task<SbomReportDto?> GetSbomReportDtoByUid(Guid uid);
}
