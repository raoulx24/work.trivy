using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
internal interface IKubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent> :
    IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TKubernetesObject : IKubernetesObject
        where TWatcherParams : IKubernetesNamespacedWatcherParams
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    void Delete(TWatcherParams watcherParams);
}