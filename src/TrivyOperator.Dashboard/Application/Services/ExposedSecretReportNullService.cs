using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ExposedSecretReportNullService : IExposedSecretReportService
{
    public Task<IEnumerable<ExposedSecretReportDto>> GetExposedSecretReportDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null) => Task.FromResult<IEnumerable<ExposedSecretReportDto>>([]);

    public Task<IList<ExposedSecretReportDenormalizedDto>> GetExposedSecretDenormalizedDtos(
        string? namespaceName = null) => Task.FromResult<IList<ExposedSecretReportDenormalizedDto>>([]);

    public Task<IEnumerable<string>> GetActiveNamespaces() => Task.FromResult<IEnumerable<string>>([]);

    public Task<IEnumerable<ExposedSecretReportImageDto>> GetExposedSecretReportImageDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null) => Task.FromResult<IEnumerable<ExposedSecretReportImageDto>>([]);

    public Task<IEnumerable<EsSeveritiesByNsSummaryDto>> GetExposedSecretReportSummaryDtos() =>
        Task.FromResult<IEnumerable<EsSeveritiesByNsSummaryDto>>([]);
}
