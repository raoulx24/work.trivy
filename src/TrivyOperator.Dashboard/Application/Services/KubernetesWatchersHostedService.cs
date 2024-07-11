using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesWatchersHostedService(
    IServiceProvider serviceProvider,
    ILogger<KubernetesWatchersHostedService> logger) : BackgroundService, IHostedService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Kubernetes Watchers Hosted Service service running.");

        var x = serviceProvider.GetServices<IKubernetesClusterScopedWatcher>();


        
        while (!cancellationToken.IsCancellationRequested)
        {
            //await ProcessChannelMessage(cancellationToken);
        }
    }
}
