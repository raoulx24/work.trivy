using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public interface IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>
    where TKubernetesObject : IKubernetesObject
    where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
    where TWatcherParams : IKubernetesWatcherParams
    where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>

{
    void Add(TWatcherParams watcherParams);
}