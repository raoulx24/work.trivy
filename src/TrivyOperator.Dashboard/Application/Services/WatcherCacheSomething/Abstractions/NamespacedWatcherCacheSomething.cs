using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class
    NamespacedWatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher,
        TKubernetesObject,
        TKubernetesObjectList>(
        TCacheRefresh cacheRefresh,
        TKubernetesWatcher kubernetesWatcher,
        ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher,
            TKubernetesObject, TKubernetesObjectList>> logger)
    : WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher,
            TKubernetesObject, TKubernetesObjectList>(cacheRefresh, kubernetesWatcher, logger),
        INamespacedWatcherCacheSomething where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
    where TCacheRefresh : ICacheRefresh<TKubernetesObject, TBackgroundQueue>
    where TKubernetesWatcherEvent : class, IWatcherEvent<TKubernetesObject>, new()
    where TKubernetesWatcher : INamespacedWatcher<TKubernetesObject>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>
{
    public void StopSomething(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        Logger.LogDebug("Removing Watcher for {kubernetesObjectType}.", typeof(TKubernetesObject).Name);
        KubernetesWatcher.Delete(sourceKubernetesObject);
    }
}
