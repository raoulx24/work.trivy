using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class WatchersSomethingHostedService(
    IServiceProvider serviceProvider,
    ILogger<WatchersSomethingHostedService> logger) : BackgroundService, IHostedService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service started.");
        foreach (IClusterScopedWatcherCacheSomething kcswcs in serviceProvider.GetServices<IClusterScopedWatcherCacheSomething>())
        {
            kcswcs.StartSomething(cancellationToken);
        }

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
