using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers;

public class NamespaceWatcher : KubernetesClusterScopedWatcher<V1NamespaceList, V1Namespace, KubernetesClusterScopedWatcherParams>
{
    public NamespaceWatcher(IKubernetesClientFactory kubernetesClientFactory, ILogger<NamespaceWatcher> logger) :
        base(kubernetesClientFactory, logger)
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
