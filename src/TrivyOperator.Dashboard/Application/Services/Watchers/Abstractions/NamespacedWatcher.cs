using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class NamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>,
    INamespacedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    public NamespacedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<NamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    public void Delete(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        string sourceNamespace = GetNamespaceFromSourceEvent(sourceKubernetesObject);
        logger.LogInformation("Deleting Watcher for {kubernetesObjectType} and key {watcherKey}.", typeof(TKubernetesObject), sourceNamespace);
        if (watchers.TryGetValue(sourceNamespace, value: out TaskWithCts taskWithCts))
        {
            taskWithCts.Cts.Cancel();
            // TODO: do I have to wait for Task.IsCanceled?
            watchers.Remove(sourceNamespace);
        }
    }

    protected override async Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        TKubernetesObject kubernetesObject = new() { Metadata = new() { Name = "fakeObject", NamespaceProperty = GetNamespaceFromSourceEvent(sourceKubernetesObject) } };
        TKubernetesWatcherEvent watcherEvent = new() { KubernetesObject = kubernetesObject, WatcherEventType = WatchEventType.Error };
        
        await backgroundQueue.QueueBackgroundWorkItemAsync(watcherEvent);
    }
}
