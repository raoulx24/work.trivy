using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh;

public class NamespaceCacheRefresh : CacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>>
{
    public NamespaceCacheRefresh(IBackgroundQueue<V1Namespace> backgroundQueue,
        IConcurrentCache<string, IList<V1Namespace>> cache,
        IServiceProvider serviceProvider,
        ILogger<NamespaceCacheRefresh> logger)
        : base(backgroundQueue, cache, logger)
    {
        this.serviceProvider = serviceProvider;
    }

    protected IServiceProvider serviceProvider { get; init; }

    protected override void ProcessAddEvent(IWatcherEvent<V1Namespace> watcherEvent, CancellationToken cancellationToken)
    {
        base.ProcessAddEvent(watcherEvent, cancellationToken);
        foreach (INamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<INamespacedWatcherCacheSomething>())
        {
            knwcs.StartSomething(cancellationToken, watcherEvent.KubernetesObject);
        }
    }

    protected override void ProcessDeleteEvent(IWatcherEvent<V1Namespace> watcherEvent)
    {
        base.ProcessDeleteEvent(watcherEvent);
        foreach (INamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<INamespacedWatcherCacheSomething>())
        {
            knwcs.StopSomething(watcherEvent.KubernetesObject);
        }
    }
}
