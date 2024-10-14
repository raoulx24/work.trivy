using TrivyOperator.Dashboard.Application.Alerts;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class AlertsService(IConcurrentCache<string, IList<Alert>> cache, ILogger<AlertsService> logger) : IAlertsService
{
    public Task AddAlert(string emitter, Alert alert)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAlert(string emitter, Alert alert)
    {
        return Task.CompletedTask;
    }

    public Task<IList<AlertDto>> GetAllAlertDtos()
    {
        IEnumerable<AlertDto> result = cache
            .Where(kvp => kvp.Value.Any())
            .SelectMany(kvp => kvp.Value
                .Select(alert => alert.ToAlertDto(kvp.Key)));

        return Task.FromResult<IList<AlertDto>>([.. result]);
    }

}
