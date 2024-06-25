using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using YamlDotNet.Core.Tokens;

namespace TrivyOperator.Dashboard.Application.Services;

public class CacheRefreshHostedService(
    IServiceProvider services,
    IConcurrentCache<string, DateTime> cache,
    ILogger<KubernetesHostedService> logger) : IHostedService, IDisposable
{
    private DateTime lastExecution = DateTime.UtcNow;
    private Timer? timer = null;

    public Task StartAsync(CancellationToken stoppingTokenn)
    {
        logger.LogInformation("Cache Refresh Hosted Service running.");

        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        IDictionary<string, DateTime> workingEvents = cache
            .Where(x => x.Key.StartsWith("vulenrabilityreportcr.") && x.Value > lastExecution)
            .ToDictionary<string, DateTime>();

        DateTime newLastExecution = workingEvents.Values.Any() ? workingEvents.Values.Max(x => x) : lastExecution;

        foreach (string key in workingEvents.Keys)
        {
            using IServiceScope scope = services.CreateScope();
            foreach (IKubernetesNamespaceAddedOrModifiedHandler handler in scope.ServiceProvider
                         .GetServices<IKubernetesNamespaceAddedOrModifiedHandler>())
            {
                await handler.Handle(key.Replace("vulenrabilityreportcr.", ""));
            }
        }

        lastExecution = newLastExecution;
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache Refresh Hosted Service is stopping.");
        timer?.Change(Timeout.Infinite, 0);
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
