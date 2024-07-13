using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;

public class WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>
    : IWatcherCacheSomething
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
    where TCacheRefresh : ICacheRefresh<TKubernetesObject, TBackgroundQueue>
    where TKubernetesWatcherEvent : class, IKubernetesWatcherEvent<TKubernetesObject>, new()
    where TKubernetesWatcher : IKubernetesWatcher<TKubernetesObject>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>

{
    protected TCacheRefresh cacheRefresh { get; init; }
    protected TKubernetesWatcher kubernetesWatcher { get; init; }
    protected ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger { get; init; }

    public WatcherCacheSomething(TCacheRefresh cacheRefresh,
        TKubernetesWatcher kubernetesWatcher,
        ILogger<WatcherCacheSomething<TBackgroundQueue, TCacheRefresh, TKubernetesWatcherEvent, TKubernetesWatcher, TKubernetesObject, TKubernetesObjectList>> logger)
    {
        this.cacheRefresh = cacheRefresh;
        this.kubernetesWatcher = kubernetesWatcher;
        this.logger = logger;
    }


    public void StartSomething(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        kubernetesWatcher.Add(cancellationToken, sourceKubernetesObject);
        if (!cacheRefresh.IsQueueProcessingStarted())
        {
            cacheRefresh.StartEventsProcessing(cancellationToken);
        }
    }
}
