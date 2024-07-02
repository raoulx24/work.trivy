using k8s;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public interface IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
    where TKubernetesObject : IKubernetesObject
    where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
    where TWatcherParams : IKubernetesWatcherParams

{
    void Add(TWatcherParams watcherParams);
}