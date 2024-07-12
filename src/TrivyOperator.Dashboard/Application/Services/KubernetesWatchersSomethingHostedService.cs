using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesWatchersSomethingHostedService(
    IServiceProvider serviceProvider,
    ILogger<KubernetesWatchersSomethingHostedService> logger) : BackgroundService, IHostedService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service started.");

        IEnumerable<IKubernetesClusterScopedWatcherCacheSomething> singletons = serviceProvider.GetServices<IKubernetesClusterScopedWatcherCacheSomething>();
        
        //IServiceCollection serviceCollection = serviceProvider.GetRequiredService<IServiceCollection>();
        //List<object> singletons = serviceCollection
        //    .Where(descriptor => descriptor.Lifetime == ServiceLifetime.Singleton)
        //    .Select(descriptor => serviceProvider.GetRequiredService(descriptor.ServiceType))
        //    .ToList();

        foreach (IKubernetesClusterScopedWatcherCacheSomething kcswcs in serviceProvider.GetServices<IKubernetesClusterScopedWatcherCacheSomething>())
        {
            //if (kcswcs is IKubernetesClusterScopedWatcherCacheSomething clusterScopedSomething)
            //{
            kcswcs.StartSomething(cancellationToken);
            //}
        }

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Watcher Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
