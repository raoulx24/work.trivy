using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class SbomReportNullService() : ISbomReportService
{
    public Task<IEnumerable<SbomReportDto>> GetSbomReportDtos(string? namespaceName = null) =>
        Task.FromResult<IEnumerable<SbomReportDto>>([]);

    public Task<SbomReportDto?> GetSbomReportDtoByUid(Guid uid) =>
        Task.FromResult<SbomReportDto?>(null);

    public Task<IEnumerable<string>> GetActiveNamespaces() =>
        Task.FromResult<IEnumerable<string>>([]);
}
