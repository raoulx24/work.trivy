using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;

public class NamespaceWatcherCacheSomething : ClusterScopedWatcherCacheSomething<
    IBackgroundQueue<V1Namespace>,
    ICacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>>,
    WatcherEvent<V1Namespace>,
    IClusterScopedWatcher<V1Namespace>,
    V1Namespace,
    V1NamespaceList>
{
    public NamespaceWatcherCacheSomething(
        ICacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>> cacheRefresh,
        IClusterScopedWatcher<V1Namespace> kubernetesWatcher,
        ILogger<NamespaceWatcherCacheSomething> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }
}
