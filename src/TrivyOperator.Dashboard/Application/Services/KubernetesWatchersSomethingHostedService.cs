using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesWatchersSomethingHostedService(
    IServiceProvider serviceProvider,
    ILogger<KubernetesWatchersSomethingHostedService> logger) : BackgroundService, IHostedService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service started.");
        foreach (IKubernetesClusterScopedWatcherCacheSomething kcswcs in serviceProvider.GetServices<IKubernetesClusterScopedWatcherCacheSomething>())
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
