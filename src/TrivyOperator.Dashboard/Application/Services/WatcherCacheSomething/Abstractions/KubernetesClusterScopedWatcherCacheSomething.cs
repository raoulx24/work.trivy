using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class KubernetesClusterScopedWatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>
    : WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>,
      IKubernetesClusterScopedWatcherCacheSomething
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
        where TCacheRefresh : ICacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
        where TKubernetesWatcherEvent : class, IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TKubernetesWatcher : IKubernetesClusterScopedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
        where TKubernetesObjectList : IItems<TKubernetesObject>
{
    public KubernetesClusterScopedWatcherCacheSomething(TCacheRefresh cacheRefresh, TKubernetesWatcher kubernetesWatcher, ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }
}
