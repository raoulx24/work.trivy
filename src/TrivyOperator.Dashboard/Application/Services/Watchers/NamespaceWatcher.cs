using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class NamespaceWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<V1Namespace> backgroundQueue,
    ILogger<NamespaceWatcher> logger)
    : ClusterScopedWatcher<V1NamespaceList, V1Namespace, IBackgroundQueue<V1Namespace>, WatcherEvent<V1Namespace>>(
        kubernetesClientFactory,
        backgroundQueue,
        logger)
{
    protected override async Task<HttpOperationResponse<V1NamespaceList>> GetKubernetesObjectWatchList(
        IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
        CancellationToken cancellationToken) => await KubernetesClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
        watch: true,
        timeoutSeconds: int.MaxValue,
        cancellationToken: cancellationToken);
}
