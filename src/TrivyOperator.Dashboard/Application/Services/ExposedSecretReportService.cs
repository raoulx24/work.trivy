using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ExposedSecretReportService(IConcurrentCache<string, IList<ExposedSecretReportCr>> cache) : IExposedSecretReportService
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

    public Task<IEnumerable<EsSeveritiesByNsSummaryDto>> GetExposedSecretReportSummaryDtos()
    {
        List<EsSeveritiesByNsSummaryDto> summaryDtos = [];
        List<EsSeveritiesByNsSummaryDetailDto> detailDtos = [];
        EsSeveritiesByNsSummaryDto summaryDto;
        int[] severityIds = Enum.GetValues(typeof(TrivySeverity)).Cast<int>().Where(x => x < 4).ToArray();
        summaryDtos = cache
            .Where(kvp => kvp.Value.Any())
            .SelectMany(kvp => kvp.Value
                .SelectMany(es => (es.Report?.Secrets ?? [])
                    .Select(esd => new { es.Metadata.NamespaceProperty, esd.Severity, esd.RuleId })))
            .GroupBy(item => new { item.NamespaceProperty, item.Severity })
            .Select(group => new
            {
                namespaceName = group.Key.NamespaceProperty,
                trivySeverityId = group.Key.Severity,
                totalCount = group.Count(),
                distinctCount = group.Select(item => item.RuleId).Distinct().Count(),
            })
            .GroupBy(x => x.namespaceName)
            .SelectMany(g => severityIds.Select(SeverityId => new
            {
                NamespaceName = g.Key,
                SeverityId,
                TotalCount = g.FirstOrDefault(x => (int)x.trivySeverityId == SeverityId)?.totalCount ?? 0,
                DistinctCount = g.FirstOrDefault(x => (int)x.trivySeverityId == SeverityId)?.distinctCount ?? 0,
            }))
            .GroupBy(last => last.NamespaceName)
            .Select(summaryGroup =>
            {
                EsSeveritiesByNsSummaryDto essns = new()
                {
                    Uid = Guid.NewGuid(),
                    NamespaceName = summaryGroup.Key,
                    Details = summaryGroup.Select(detail => {
                        EsSeveritiesByNsSummaryDetailDto detailDto = new()
                        {
                            Id = detail.SeverityId,
                            TotalCount = detail.TotalCount,
                            DistinctCount = detail.DistinctCount,
                        };
                        return detailDto;
                    }).ToList(),
                    IsTotal = false,
                };
                return essns;
            }).ToList();
        var totalSumary = cache
            .Where(kvp => kvp.Value.Any())
            .SelectMany(kvp => kvp.Value
                .SelectMany(es => (es.Report?.Secrets ?? [])
                    .Select(esd => new { esd.Severity, esd.RuleId })))
            .GroupBy(item => item.Severity)
            .Select(group => new EsSeveritiesByNsSummaryDetailDto
            {
                Id = (int)group.Key,
                TotalCount = group.Count(),
                DistinctCount = group.Select(item => item.RuleId).Distinct().Count(),
            });

        detailDtos = totalSumary
            .Concat(severityIds
                .Where(id => !totalSumary.Any(x => x.Id == id))
                .Select(id => new EsSeveritiesByNsSummaryDetailDto 
                {
                    Id = id, TotalCount = 0, DistinctCount = 0, 
                })
            ).ToList();
        summaryDto = new()
        {
            Uid = Guid.NewGuid(),
            NamespaceName = string.Empty,
            Details = detailDtos,
            IsTotal = true,
        };
        summaryDtos.Add(summaryDto);

        return Task.FromResult<IEnumerable<EsSeveritiesByNsSummaryDto>>([.. summaryDtos.OrderBy(x => x.NamespaceName)]);

    }
}
