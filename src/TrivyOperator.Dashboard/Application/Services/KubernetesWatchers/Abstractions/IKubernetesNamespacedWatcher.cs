using k8s;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
internal interface IKubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
    where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
    where TWatcherParams : IKubernetesNamespacedWatcherParams
{
    void Delete(TWatcherParams watcherParams);
}