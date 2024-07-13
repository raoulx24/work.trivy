using k8s.Models;
using System.Threading;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
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

    protected override void ProcessAddEvent(IKubernetesWatcherEvent<V1Namespace> watcherEvent, CancellationToken cancellationToken)
    {
        base.ProcessAddEvent(watcherEvent, cancellationToken);
        foreach (IKubernetesNamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<IKubernetesNamespacedWatcherCacheSomething>())
        {
            knwcs.StartSomething(cancellationToken, watcherEvent.KubernetesObject);
        }
    }

    protected override void ProcessDeleteEvent(IKubernetesWatcherEvent<V1Namespace> watcherEvent)
    {
        base.ProcessDeleteEvent(watcherEvent);
        foreach (IKubernetesNamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<IKubernetesNamespacedWatcherCacheSomething>())
        {
            knwcs.StopSomething(watcherEvent.KubernetesObject);
        }
    }
}
