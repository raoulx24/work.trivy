using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
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
        // TODO foreach singletons of type IKubernetesNamespacedWatcher, call Add
        //KubernetesNamespacedWatcherParams watcherParams = new() 
        //    { CancellationToken = cancellationToken, kubernetesNamespace = eventNamespaceName };
    }

    protected override void ProcessDeleteEvent(KubernetesWatcherEvent<V1Namespace> watcherEvent)
    {
        base.ProcessDeleteEvent(watcherEvent);
        // TODO foreach singletons of type IKubernetesNamespacedWatcher, call Delete
        //KubernetesNamespacedWatcherParams watcherParams = new()
        //{ CancellationToken = new(), kubernetesNamespace = eventNamespaceName };
    }
}
