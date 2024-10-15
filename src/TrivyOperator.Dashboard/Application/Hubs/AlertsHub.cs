using Microsoft.AspNetCore.SignalR;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Hubs;

public class AlertsHub(IAlertsService alertsService, 
    ILogger<AlertsHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogDebug("New client connected to Hub.");
        IList<AlertDto> items = await alertsService.GetAlertDtos();
        await Clients.Caller.SendAsync("ReceiveAddedAlert", items);

        await base.OnConnectedAsync();
    }
}
