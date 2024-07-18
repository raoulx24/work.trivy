using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;

public class NamespaceCacheWatcherEventHandler(
    ICacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>> cacheRefresh,
    IClusterScopedWatcher<V1Namespace> kubernetesWatcher,
    ILogger<NamespaceCacheWatcherEventHandler> logger)
    : ClusterScopedCacheWatcherEventHandler<IBackgroundQueue<V1Namespace>,
        ICacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>>, WatcherEvent<V1Namespace>,
        IClusterScopedWatcher<V1Namespace>, V1Namespace, V1NamespaceList>(cacheRefresh, kubernetesWatcher, logger);
