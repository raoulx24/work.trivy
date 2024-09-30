using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherState;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class
    NamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        IWatcherState watcherState,
        ILogger<NamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>>
            logger)
    : KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        kubernetesClientFactory,
        backgroundQueue,
        watcherState,
        logger), INamespacedWatcher<TKubernetesObject>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
    where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
    where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    public void Delete(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        string sourceNamespace = GetNamespaceFromSourceEvent(sourceKubernetesObject);
        Logger.LogInformation(
            "Deleting Watcher for {kubernetesObjectType} and key {watcherKey}.",
            typeof(TKubernetesObject).Name,
            sourceNamespace);
        if (Watchers.TryGetValue(sourceNamespace, out TaskWithCts taskWithCts))
        {
            taskWithCts.Cts.Cancel();
            // TODO: do I have to wait for Task.IsCanceled?
            Watchers.Remove(sourceNamespace);
        }
    }

    protected override async Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        TKubernetesObject kubernetesObject = new()
        {
            Metadata = new V1ObjectMeta
            {
                Name = "fakeObject", NamespaceProperty = GetNamespaceFromSourceEvent(sourceKubernetesObject),
            },
        };
        TKubernetesWatcherEvent watcherEvent =
            new() { KubernetesObject = kubernetesObject, WatcherEventType = WatchEventType.Error };

        await BackgroundQueue.QueueBackgroundWorkItemAsync(watcherEvent);
    }
}
