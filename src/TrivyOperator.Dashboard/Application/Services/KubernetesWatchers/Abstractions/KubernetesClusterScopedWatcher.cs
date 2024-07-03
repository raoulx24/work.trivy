using k8s;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>,
    IKubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesClusterScopedWatcherParams
{
    public KubernetesClusterScopedWatcher(IKubernetesClientFactory kubernetesClientFactory, ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>> logger)
        : base(kubernetesClientFactory, logger)
    {
    }

    protected override void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams)
    {
        watchers.Add("genericKey", watcher);
    }
}
