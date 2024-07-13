using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class ClusterScopedWatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>
    : WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>,
      IClusterScopedWatcherCacheSomething
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
        where TCacheRefresh : ICacheRefresh<TKubernetesObject, TBackgroundQueue>
        where TKubernetesWatcherEvent : class, IWatcherEvent<TKubernetesObject>, new()
        where TKubernetesWatcher : IClusterScopedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
        where TKubernetesObjectList : IItems<TKubernetesObject>
{
    public ClusterScopedWatcherCacheSomething(TCacheRefresh cacheRefresh, TKubernetesWatcher kubernetesWatcher, ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }
}
