using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class CacheRefreshHostedService(
    IConcurrentCache<string, DateTime> cache,
    IKubernetesNamespaceAddedHandler kubernetesNamespaceAddedHandler,
    ILogger<KubernetesHostedService> logger) : BackgroundService
{
    private DateTime lastExecution { get; } = DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(60000);

            IEnumerable<string> k8sNamespaces = cache.Where(x => x.Key.StartsWith("vulenrabilityreportcr."))
                .Where(x => x.Value > lastExecution)
                .Select(x => x.Key.Replace("vulenrabilityreportcr.", ""));

            foreach (string k8snamespace in k8sNamespaces)
            {
                await kubernetesNamespaceAddedHandler.Handle(k8snamespace);
            }
        }
    }
}
