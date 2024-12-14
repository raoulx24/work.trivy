using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class CacheWatcherEventHandlerHostedService(
    IEnumerable<IClusterScopedCacheWatcherEventHandler> services,
    ILogger<CacheWatcherEventHandlerHostedService> logger) : BackgroundService
{
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service started.");
        foreach (IClusterScopedCacheWatcherEventHandler service in services)
        {
            service.Start(cancellationToken);
        }

        return Task.CompletedTask;
    }
}
