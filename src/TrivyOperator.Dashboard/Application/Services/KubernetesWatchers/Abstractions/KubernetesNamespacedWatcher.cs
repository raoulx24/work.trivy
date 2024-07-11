﻿using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>,
    IKubernetesNamespacedWatcher
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TSourceKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    public KubernetesNamespacedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    public void Delete(TSourceKubernetesObject? sourceKubernetesObject)
    {
        string sourceNamespace = VarUtils.GetWatchersKey(sourceKubernetesObject);
        if (watchers.TryGetValue(sourceNamespace, value: out TaskWithCts taskWithCts))
        {
            taskWithCts.Cts.Cancel();
            // TODO: do I have to wait for Task.IsCanceled?
            watchers.Remove(sourceNamespace);
        }
    }

    protected override async Task EnqueueWatcherEventWithError(TSourceKubernetesObject? sourceKubernetesObject)
    {
        TKubernetesObject kubernetesObject = new() { Metadata = new() { Name = "fakeObject", NamespaceProperty = VarUtils.GetWatchersKey(sourceKubernetesObject) } };

        await backgroundQueue.QueueBackgroundWorkItemAsync(new() { KubernetesObject = kubernetesObject, WatcherEvent = WatchEventType.Error });
    }
}
