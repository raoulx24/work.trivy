using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>,
    IKubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesClusterScopedWatcherParams
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgoundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    public KubernetesClusterScopedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        IBackgroundQueue<IKubernetesWatcherEvent<TKubernetesObject>, TKubernetesObject> backgroundQueue,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    protected override void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams)
    {
        watchers.Add("genericKey", watcher);
    }
}
