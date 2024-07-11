using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class KubernetesClusterScopedWatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject>
    : WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject>, IKubernetesClusterScopedWatcherCacheSomething
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
        where TCacheRefresh : ICacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
        where TKubernetesWatcherEvent : class, IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TKubernetesWatcher : IKubernetesWatcher<IItems<TKubernetesObject>, TKubernetesObject, IKubernetesObject<V1ObjectMeta>, TBackgroundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
{
    public KubernetesClusterScopedWatcherCacheSomething(TCacheRefresh cacheRefresh, TKubernetesWatcher kubernetesWatcher, ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject>> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }
}
