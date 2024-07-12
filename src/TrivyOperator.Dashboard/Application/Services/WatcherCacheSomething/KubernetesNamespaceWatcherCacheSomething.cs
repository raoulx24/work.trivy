using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;

public class KubernetesNamespaceWatcherCacheSomething : KubernetesClusterScopedWatcherCacheSomething<
    IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>,
    ICacheRefresh<V1Namespace, KubernetesWatcherEvent<V1Namespace>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>>,
    KubernetesWatcherEvent<V1Namespace>,
    IKubernetesWatcher<V1NamespaceList, V1Namespace, IKubernetesObject<V1ObjectMeta>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>, KubernetesWatcherEvent<V1Namespace>>,
    V1Namespace,
    V1NamespaceList>
{
    public KubernetesNamespaceWatcherCacheSomething(
        ICacheRefresh<V1Namespace, KubernetesWatcherEvent<V1Namespace>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>> cacheRefresh,
        IKubernetesWatcher<V1NamespaceList, V1Namespace, IKubernetesObject<V1ObjectMeta>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>, KubernetesWatcherEvent<V1Namespace>> kubernetesWatcher,
        ILogger<KubernetesNamespaceWatcherCacheSomething> logger) : base(cacheRefresh, kubernetesWatcher, logger)
    {
    }
}
