using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;

public class KubernetesNamespacedWatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>
    : WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>,
      IKubernetesNamespacedWatcherCacheSomething
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
        where TCacheRefresh : ICacheRefresh<TKubernetesObject, TBackgroundQueue>
        where TKubernetesWatcherEvent : class, IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TKubernetesWatcher : IKubernetesNamespacedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
        where TKubernetesObjectList : IItems<TKubernetesObject>
{
    public KubernetesNamespacedWatcherCacheSomething(TCacheRefresh cacheRefresh, TKubernetesWatcher kubernetesWatcher, ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }

    public void StopSomething(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        logger.LogDebug("Removing Watcher for {kubernetesObjectType}.", typeof(TKubernetesObject).Name);
        kubernetesWatcher.Delete(sourceKubernetesObject);
    }
}
