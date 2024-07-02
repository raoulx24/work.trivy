using k8s;
using k8s.Autorest;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class KubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams> :
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>,
    IKubernetesNamespacedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesNamespacedWatcherParams
{
    public KubernetesNamespacedWatcher(IK8sClientFactory k8SClientFactory, ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>> logger)
        : base(k8SClientFactory, logger)
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
