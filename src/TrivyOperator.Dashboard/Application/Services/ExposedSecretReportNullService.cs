using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ExposedSecretReportNullService() : IExposedSecretReportService
{
    public Task<IEnumerable<ExposedSecretReportDto>> GetExposedSecretReportDtos(string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    { return Task.FromResult<IEnumerable<ExposedSecretReportDto>>([]); }

    public Task<IList<ExposedSecretReportDenormalizedDto>> GetExposedSecretDenormalizedDtos(string? namespaceName = null)
    { return Task.FromResult<IList<ExposedSecretReportDenormalizedDto>>([]); }

    public Task<IEnumerable<string>> GetActiveNamespaces()
    { return Task.FromResult<IEnumerable<string>>([]);
    }

    public Task<IEnumerable<ExposedSecretReportImageDto>> GetExposedSecretReportImageDtos(
        string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    { return Task.FromResult<IEnumerable<ExposedSecretReportImageDto>>([]); }

    public Task<IEnumerable<EsSeveritiesByNsSummaryDto>> GetExposedSecretReportSummaryDtos()
    { return Task.FromResult<IEnumerable<EsSeveritiesByNsSummaryDto>>([]); }
}
