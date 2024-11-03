using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class
    ClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        IServiceProvider serviceProvider,
        ILogger<ClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue,
            TKubernetesWatcherEvent>> logger)
    : KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        kubernetesClientFactory,
        backgroundQueue,
        serviceProvider,
        logger), IClusterScopedWatcher<TKubernetesObject>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
    where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
    where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    protected override async Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        TKubernetesObject kubernetesObject = new();
        WatcherEvent<TKubernetesObject> watcherEvent =
            new() { KubernetesObject = kubernetesObject, WatcherEventType = WatchEventType.Error };

        await BackgroundQueue.QueueBackgroundWorkItemAsync(watcherEvent);
    }
}
