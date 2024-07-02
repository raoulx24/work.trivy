using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.WatcherParams;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class NamespaceWatcher : KubernetesClusterScopedWatcher<V1NamespaceList, V1Namespace, KubernetesClusterScopedWatcherParams>
{
    public NamespaceWatcher(IK8sClientFactory k8SClientFactory, ILogger<NamespaceWatcher> logger) :
        base(k8SClientFactory, logger)
    {
    }

    protected override async Task<HttpOperationResponse<V1NamespaceList>> GetKubernetesObjectWatchList(KubernetesClusterScopedWatcherParams watcherParams, CancellationToken cancellationToken)
    {
        return await kubernetesClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
                        watch: true,
                        timeoutSeconds: int.MaxValue,
                        cancellationToken: cancellationToken);
    }
}
