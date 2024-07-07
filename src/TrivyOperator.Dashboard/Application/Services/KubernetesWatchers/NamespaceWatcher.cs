using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers;

public class NamespaceWatcher : KubernetesWatcher<V1NamespaceList, V1Namespace, IKubernetesObject<V1ObjectMeta>, BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>, KubernetesWatcherEvent<V1Namespace>>
{
    public NamespaceWatcher(IKubernetesClientFactory kubernetesClientFactory,
        BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace> backgroundQueue,
        ILogger<NamespaceWatcher> logger) :
        base(kubernetesClientFactory, backgroundQueue, logger)
    { }

    protected override async Task<HttpOperationResponse<V1NamespaceList>> GetKubernetesObjectWatchList(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject, CancellationToken cancellationToken)
    {
        return await kubernetesClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
                        watch: true,
                        timeoutSeconds: int.MaxValue,
                        cancellationToken: cancellationToken);
    }

    protected override async Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        V1Namespace v1Namespace = new();

        await backgroundQueue.QueueBackgroundWorkItemAsync(new() { KubernetesObject = v1Namespace, WatcherEvent = WatchEventType.Error });
    }
}
