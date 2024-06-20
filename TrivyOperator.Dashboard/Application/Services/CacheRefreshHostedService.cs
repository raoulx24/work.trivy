using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class CacheRefreshHostedService(
    IServiceProvider services,
    IConcurrentCache<string, DateTime> cache,
    ILogger<KubernetesHostedService> logger) : BackgroundService
{
    private readonly DateTime lastExecution = DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache Refresh Hosted Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(60000, stoppingToken);

            IEnumerable<string> k8sNamespaces = cache.Where(x => x.Key.StartsWith("vulenrabilityreportcr."))
                .Where(x => x.Value > lastExecution)
                .Select(x => x.Key.Replace("vulenrabilityreportcr.", ""));
            foreach (string k8sNamespace in k8sNamespaces)
            {
                using IServiceScope scope = services.CreateScope();
                foreach (IKubernetesNamespaceAddedHandler handler in scope.ServiceProvider
                             .GetServices<IKubernetesNamespaceAddedHandler>())
                {
                    await handler.Handle(k8sNamespace);
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache Refresh Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
