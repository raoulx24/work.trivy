using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ExposedSecretReportService(IConcurrentCache<string, IList<ExposedSecretReportCr>> cache, ILogger<ExposedSecretReportService> logger) : IExposedSecretReportService
{
    public Task<IEnumerable<ExposedSecretReportDto>> GetExposedSecretReportDtos(string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities ??= [];
        int[] incudedSeverities = ((int[])Enum.GetValues(typeof(TrivySeverity))).ToList().Except(excludedSeverities).ToArray();

        IEnumerable<ExposedSecretReportDto> dtos = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value
                .Select(cr => cr.ToExposedSecretReportDto())
                .Select(dto =>
                {
                    dto.Details = dto.Details
                        .Join(incudedSeverities, vulnerability => vulnerability.SeverityId, id => id, (vulnerability, id) => vulnerability)
                        .ToArray();
                    return dto;
                })
                .Where(dto => !excludedSeverities.Any() || dto.Details.Length != 0)
            );

        return Task.FromResult<IEnumerable<ExposedSecretReportDto>>(dtos);
    }

    public Task<IList<ExposedSecretReportDenormalizedDto>> GetExposedSecretDenormalizedDtos(string? namespaceName = null)
    {
        List<ExposedSecretReportDenormalizedDto> result = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value)
            .SelectMany(car => car.ToExposedSecretReportDenormalizedDtos())
            .ToList();

        return Task.FromResult<IList<ExposedSecretReportDenormalizedDto>>(result);
    }

    public Task<IEnumerable<string>> GetActiveNamespaces()
    {
        return Task.FromResult(cache.Where(x => x.Value.Any()).Select(x => x.Key));
    }

    public Task<IEnumerable<ExposedSecretReportImageDto>> GetExposedSecretReportImageDtos(
        string? namespaceName = null, IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities ??= [];
        int[] incudedSeverities = ((int[])Enum.GetValues(typeof(TrivySeverity))).ToList().Except(excludedSeverities).ToArray();

        IEnumerable<ExposedSecretReportImageDto> exposedSecretReportImageDtos = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value
                .GroupBy(esr => esr.Report?.Artifact?.Digest)
                .Select(group => group.ToExposedSecretReportImageDto())
                .Select(esrDto =>
                {
                    esrDto.Details = esrDto.Details
                        .Join(incudedSeverities, vulnerability => vulnerability.SeverityId, id => id, (vulnerability, id) => vulnerability)
                        .ToArray();
                    return esrDto;
                })
                .Where(esrDto => !excludedSeverities.Any() || esrDto.Details.Length != 0)
            );

        return Task.FromResult(exposedSecretReportImageDtos);
    }
}
