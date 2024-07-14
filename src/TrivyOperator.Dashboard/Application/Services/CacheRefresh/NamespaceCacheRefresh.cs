using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacheRefresh;

public class NamespaceCacheRefresh(
    IBackgroundQueue<V1Namespace> backgroundQueue,
    IConcurrentCache<string, IList<V1Namespace>> cache,
    IServiceProvider serviceProvider,
    ILogger<NamespaceCacheRefresh> logger)
    : CacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>>(backgroundQueue, cache, logger)
{
    protected readonly IServiceProvider ServiceProvider = serviceProvider;

    protected override void ProcessAddEvent(
        IWatcherEvent<V1Namespace> watcherEvent,
        CancellationToken cancellationToken)
    {
        base.ProcessAddEvent(watcherEvent, cancellationToken);
        foreach (INamespacedWatcherCacheSomething knwcs in
                 ServiceProvider.GetServices<INamespacedWatcherCacheSomething>())
        {
            knwcs.StartSomething(cancellationToken, watcherEvent.KubernetesObject);
        }
    }

    protected override void ProcessDeleteEvent(IWatcherEvent<V1Namespace> watcherEvent)
    {
        base.ProcessDeleteEvent(watcherEvent);
        foreach (INamespacedWatcherCacheSomething knwcs in
                 ServiceProvider.GetServices<INamespacedWatcherCacheSomething>())
        {
            knwcs.StopSomething(watcherEvent.KubernetesObject);
        }
    }
}
