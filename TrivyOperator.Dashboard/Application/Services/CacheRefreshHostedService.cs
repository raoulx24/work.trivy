using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class CacheRefreshHostedService(
    IServiceProvider services,
    IConcurrentCache<string, DateTime> cache,
    ILogger<KubernetesHostedService> logger) : BackgroundService
{
    private DateTime lastExecution = DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache Refresh Hosted Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(60000, stoppingToken);

            DateTime[] lastMoments = cache.Where(x => x.Key.StartsWith("vulenrabilityreportcr."))
                .Where(x => x.Value > lastExecution)
                .Select(x => x.Value)
                .ToArray();
            DateTime newLastExecution = lastMoments.Length != 0 ? lastMoments.Max(x => x) : lastExecution;

            IEnumerable<string> k8sNamespaces = cache
                .Where(x => x.Key.StartsWith("vulenrabilityreportcr.") && x.Value > lastExecution)
                .Select(x => x.Key.Replace("vulenrabilityreportcr.", ""))
                .Distinct();
            foreach (string k8sNamespace in k8sNamespaces)
            {
                using IServiceScope scope = services.CreateScope();
                foreach (IKubernetesNamespaceAddedOrModifiedHandler handler in scope.ServiceProvider
                             .GetServices<IKubernetesNamespaceAddedOrModifiedHandler>())
                {
                    await handler.Handle(k8sNamespace);
                }
            }

            lastExecution = newLastExecution;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache Refresh Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
