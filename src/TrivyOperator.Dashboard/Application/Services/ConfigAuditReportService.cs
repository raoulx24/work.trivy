using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class ConfigAuditReportService(IConcurrentCache<string, IList<ConfigAuditReportCr>> cache)
    : IConfigAuditReportService
{
    public Task<IEnumerable<ConfigAuditReportDto>> GetConfigAuditReportDtos(
        string? namespaceName = null,
        IEnumerable<int>? excludedSeverities = null)
    {
        excludedSeverities ??= [];
        int[] excludedSeveritiesArray = excludedSeverities.ToArray();
        int[] includedSeverities = ((int[])Enum.GetValues(typeof(TrivySeverity))).ToList()
            .Except(excludedSeveritiesArray)
            .ToArray();

        IEnumerable<ConfigAuditReportDto> dtos = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(
                kvp => kvp.Value.Select(cr => cr.ToConfigAuditReportDto())
                    .Select(
                        dto =>
                        {
                            dto.Details = dto.Details.Join(
                                    includedSeverities,
                                    vulnerability => vulnerability.SeverityId,
                                    id => id,
                                    (vulnerability, _) => vulnerability)
                                .ToArray();
                            return dto;
                        })
                    .Where(dto => !excludedSeveritiesArray.Any() || dto.Details.Length != 0));

        return Task.FromResult(dtos);
    }

    public Task<IList<ConfigAuditReportDenormalizedDto>> GetConfigAuditReportDenormalizedDtos(
        string? namespaceName = null)
    {
        List<ConfigAuditReportDenormalizedDto> result = cache
            .Where(kvp => string.IsNullOrEmpty(namespaceName) || kvp.Key == namespaceName)
            .SelectMany(kvp => kvp.Value)
            .SelectMany(car => car.ToConfigAuditReportDetailDenormalizedDtos())
            .ToList();

        return Task.FromResult<IList<ConfigAuditReportDenormalizedDto>>(result);
    }

    public Task<IEnumerable<string>> GetActiveNamespaces() =>
        Task.FromResult(cache.Where(x => x.Value.Any()).Select(x => x.Key));

    public Task<IEnumerable<ConfigAuditReportSummaryDto>> GetConfigAuditReportSummaryDtos()
    {
        var valuesByNs = cache.Where(kvp => kvp.Value.Any())
            .SelectMany(
                kvp => kvp.Value.Select(car => car.ToConfigAuditReportDto())
                    .SelectMany(
                        dto => dto.Details.Select(
                            detail => new
                            {
                                NamespaceName = kvp.Key,
                                Kind = dto.ResourceKind,
                                detail.SeverityId,
                                detail.CheckId,
                            }))
                    .GroupBy(key => new { key.NamespaceName, key.Kind, key.SeverityId })
                    .Select(
                        group => new
                        {
                            ns = group.Key.NamespaceName,
                            kind = group.Key.Kind,
                            severityId = group.Key.SeverityId,
                            totalCount = group.Count(),
                            distinctCount = group.Select(x => x.CheckId).Distinct().Count(),
                        }))
            .ToArray();

        string[] allKinds = valuesByNs.Select(x => x.kind).Distinct().ToArray();
        int[] allSeverities = Enum.GetValues(typeof(TrivySeverity)).Cast<int>().Where(x => x < 4).ToArray();

        var allCombinationsWithNs = cache.Where(kvp => kvp.Value.Any())
            .Select(kvp => kvp.Key)
            .SelectMany(_ => allKinds, (ns, kind) => new { ns, kind })
            .SelectMany(_ => allSeverities, (nk, severityId) => new { nk.ns, nk.kind, severityId });

        List<ConfigAuditReportSummaryDto> configAuditReportSummaryDtos = allCombinationsWithNs.GroupJoin(
                valuesByNs,
                combo => new { combo.ns, combo.kind, combo.severityId },
                count => new { count.ns, count.kind, count.severityId },
                (combo, countGroup) =>
                {
                    var countGroupArray = countGroup.ToArray();
                    return new ConfigAuditReportSummaryDto
                    {
                        NamespaceName = combo.ns,
                        Kind = combo.kind,
                        SeverityId = combo.severityId,
                        TotalCount = countGroupArray.FirstOrDefault()?.totalCount ?? 0,
                        DistinctCount = countGroupArray.FirstOrDefault()?.distinctCount ?? 0,
                    };
                })
            .ToList();
        List<ConfigAuditReportSummaryDto> result = configAuditReportSummaryDtos;

        var allConbinationsForTotals = allKinds.SelectMany(
            _ => allSeverities,
            (kind, severityId) => new { kind, severityId });

        var valueTotals = cache.Where(kvp => kvp.Value.Any())
            .SelectMany(kvp => kvp.Value.Select(car => car.ToConfigAuditReportDto()))
            .SelectMany(
                dto => dto.Details.Select(detail => new { Kind = dto.ResourceKind, detail.SeverityId, detail.CheckId }))
            .GroupBy(key => new { key.Kind, key.SeverityId })
            .Select(
                group => new
                {
                    kind = group.Key.Kind,
                    severityId = group.Key.SeverityId,
                    totalCount = group.Count(),
                    distinctCount = group.Select(x => x.CheckId).Distinct().Count(),
                });

        List<ConfigAuditReportSummaryDto> resultsTotal = allConbinationsForTotals.GroupJoin(
                valueTotals,
                combo => new { combo.kind, combo.severityId },
                count => new { count.kind, count.severityId },
                (combo, countGroup) =>
                {
                    var countGroupArray = countGroup.ToArray();
                    return new ConfigAuditReportSummaryDto
                    {
                        NamespaceName = string.Empty,
                        Kind = combo.kind,
                        SeverityId = combo.severityId,
                        TotalCount = countGroupArray.FirstOrDefault()?.totalCount ?? 0,
                        DistinctCount = countGroupArray.FirstOrDefault()?.distinctCount ?? 0,
                    };
                })
            .ToList();

        result.AddRange(resultsTotal);

        return Task.FromResult<IEnumerable<ConfigAuditReportSummaryDto>>(result);
    }
}
