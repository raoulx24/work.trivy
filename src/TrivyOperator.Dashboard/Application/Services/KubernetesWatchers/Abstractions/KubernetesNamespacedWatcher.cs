using k8s;
using k8s.Autorest;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>,
    IKubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesNamespacedWatcherParams
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    public KubernetesNamespacedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    public void Delete(TWatcherParams watcherParams)
    {
        if (watchers.TryGetValue(watcherParams.kubernetesNamespace, value: out TaskWithCts taskWithCts))
        {
            taskWithCts.Cts.Cancel();
            // TODO: do I have to wait for Task.IsCanceled?
            watchers.Remove(watcherParams.kubernetesNamespace);
        }
    }

    protected override void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams)
    {
        watchers.Add(watcherParams.kubernetesNamespace, watcher);
    }
}
