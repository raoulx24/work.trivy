using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>,
    IKubernetesNamespacedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    public KubernetesNamespacedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    public void Delete(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        string sourceNamespace = GetNamespaceFromSourceEvent(sourceKubernetesObject);
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
        TKubernetesWatcherEvent watcherEvent = new() { KubernetesObject = kubernetesObject, WatcherEvent = WatchEventType.Error };
        
        await backgroundQueue.QueueBackgroundWorkItemAsync(watcherEvent);
    }
}
