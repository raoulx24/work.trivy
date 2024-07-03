using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
using TrivyOperator.Dashboard.Application.Services.WatcherParams;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers;

public class NamespaceWatcher : KubernetesClusterScopedWatcher<V1NamespaceList, V1Namespace, KubernetesClusterScopedWatcherParams, KubernetesNamespaceBackgroundQueue, KubernetesNamespaceWatcherEvent>
{
    public NamespaceWatcher(IKubernetesClientFactory kubernetesClientFactory,
        IKubernetesNamespaceBackgroundQueue backgroundQueue,
        ILogger<NamespaceWatcher> logger) :
        base(kubernetesClientFactory, backgroundQueue, logger)
    {
    }

    protected override async Task<HttpOperationResponse<V1NamespaceList>> GetKubernetesObjectWatchList(KubernetesClusterScopedWatcherParams watcherParams, CancellationToken cancellationToken)
    {
        return await kubernetesClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
                        watch: true,
                        timeoutSeconds: int.MaxValue,
                        cancellationToken: cancellationToken);
    }

    protected override KubernetesNamespaceWatcherEvent GetKubernetesWatcherEventWithError(KubernetesClusterScopedWatcherParams watcherParams)
    {
        //throw new NotImplementedException();

        V1Namespace v1Namespace = new();

        return new KubernetesNamespaceWatcherEvent() { KubernetesObject = v1Namespace, WatcherEvent = WatchEventType.Error };
    }
}
