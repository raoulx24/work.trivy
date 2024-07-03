using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public interface IKubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>
    : IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesClusterScopedWatcherParams
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgoundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
}
