using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>(
    TCacheRefresh cacheRefresh,
    TKubernetesWatcher kubernetesWatcher,
    ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger)
    : IWatcherCacheSomething
    where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
    where TCacheRefresh : ICacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
    where TKubernetesWatcherEvent : class, IKubernetesWatcherEvent<TKubernetesObject>, new()
    where TKubernetesWatcher : IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, IKubernetesObject<V1ObjectMeta>, TBackgroundQueue, TKubernetesWatcherEvent>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>

{
    public void StartSomething(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        kubernetesWatcher.Add(cancellationToken, sourceKubernetesObject);
        if (!cacheRefresh.IsQueueProcessingStarted())
        {
            cacheRefresh.StartEventsProcessing(cancellationToken);
        }
    }
}
