using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class ClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>,
    IClusterScopedWatcher<TKubernetesObject>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    public ClusterScopedWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<ClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
        : base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }
}
