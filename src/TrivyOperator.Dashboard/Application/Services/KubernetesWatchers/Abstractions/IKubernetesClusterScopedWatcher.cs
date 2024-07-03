using k8s;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public interface IKubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
    : IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesClusterScopedWatcherParams
{
}
