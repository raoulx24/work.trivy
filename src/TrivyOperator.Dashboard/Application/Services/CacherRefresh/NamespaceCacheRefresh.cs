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

public class NamespaceCacheRefresh : CacheRefresh<V1Namespace, KubernetesWatcherEvent<V1Namespace>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>>
{
    public NamespaceCacheRefresh(IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace> backgroundQueue,
        IConcurrentCache<string, IList<V1Namespace>> cache,
        IServiceProvider serviceProvider,
        ILogger<NamespaceCacheRefresh> logger)
        : base(backgroundQueue, cache, logger)
    {
        this.serviceProvider = serviceProvider;
    }

    protected IServiceProvider serviceProvider { get; init; }

    protected override void ProcessAddEvent(KubernetesWatcherEvent<V1Namespace> watcherEvent, CancellationToken cancellationToken)
    {
        base.ProcessAddEvent(watcherEvent, cancellationToken);
        foreach (IKubernetesNamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<IKubernetesNamespacedWatcherCacheSomething>())
        {
            knwcs.StartSomething(cancellationToken, watcherEvent.KubernetesObject);
        }
    }

    protected override void ProcessDeleteEvent(KubernetesWatcherEvent<V1Namespace> watcherEvent)
    {
        base.ProcessDeleteEvent(watcherEvent);
        foreach (IKubernetesNamespacedWatcherCacheSomething knwcs in serviceProvider.GetServices<IKubernetesNamespacedWatcherCacheSomething>())
        {
            knwcs.StopSomething(watcherEvent.KubernetesObject);
        }
    }
}
