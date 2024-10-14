using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using TrivyOperator.Dashboard.Application.Alerts;
using TrivyOperator.Dashboard.Application.Hubs;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class AlertsService(IConcurrentCache<string, IList<Alert>> cache,
    IHubContext<AlertsHub> alertsHubContext,
    ILogger<AlertsService> logger) : IAlertsService
{
    public async Task AddAlert(string emitter, Alert alert)
    {
        cache.TryGetValue(emitter, value: out IList<Alert>? alerts);
        if (alerts == null)
        {
            alerts = [];
            alerts.Add(alert);
            cache.TryAdd(emitter, value: alerts);
        }
        else
        {
            alerts.Add(alert);
        }
        await alertsHubContext.Clients.All.SendAsync("ReceiveAddedAlert", alert.ToAlertDto(emitter));

        logger.LogDebug($"Added alert for {emitter} and {alert.EmitterKey} with severity {alert.Severity}.");
    }

    public async Task RemoveAlert(string emitter, Alert alert)
    {
        cache.TryGetValue(emitter, value: out IList<Alert>? alerts);
        if (alerts != null)
        {
            RemoveAlertByKey(alerts, alert.EmitterKey);
        }
        await alertsHubContext.Clients.All.SendAsync("ReceiveRemovedAlert", alert.ToAlertDto(emitter));

        logger.LogDebug($"Removed alert for {emitter} and {alert.EmitterKey}.");
    }

    public Task<IList<AlertDto>> GetAllAlertDtos()
    {
        IEnumerable<AlertDto> result = cache
            .Where(kvp => kvp.Value.Any())
            .SelectMany(kvp => kvp.Value
                .Select(alert => alert.ToAlertDto(kvp.Key)));

        return Task.FromResult<IList<AlertDto>>([.. result]);
    }

    private void RemoveAlertByKey(IList<Alert> alerts, string key)
    {
        for (int i = alerts.Count - 1; i >= 0; i--)
        {
            if (alerts[i].EmitterKey == key)
            {
                alerts.RemoveAt(i);
            }
        }
    }

}
