using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class StaticNamespaceWatcher : IClusterScopedWatcher<V1Namespace>
{
    private BackgroundQueue<WatcherEvent<V1Namespace>, V1Namespace> backgroundQueue;
    private IKubernetesNamespaceDomainService kubernetesNamespaceDomainService;
    private ILogger<StaticNamespaceWatcher> logger;

    public StaticNamespaceWatcher(BackgroundQueue<WatcherEvent<V1Namespace>, V1Namespace> backgroundQueue,
        IKubernetesNamespaceDomainService kubernetesNamespaceDomainService,
        ILogger<StaticNamespaceWatcher> logger)
    {
        this.backgroundQueue = backgroundQueue;
        this.kubernetesNamespaceDomainService = kubernetesNamespaceDomainService;
        this.logger = logger;
    }

    public async void Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObjects)
    {
        List<string> kubernetesNamespaces = await kubernetesNamespaceDomainService.GetKubernetesNamespaces();
        foreach (string kubernetesNamespace in kubernetesNamespaces)
        {
            V1Namespace v1Namespace = new() { Metadata = new() { NamespaceProperty = kubernetesNamespace } };
            WatcherEvent<V1Namespace> watcherEvent = new() { KubernetesObject = v1Namespace, WatcherEventType = WatchEventType.Added };

            await backgroundQueue.QueueBackgroundWorkItemAsync(watcherEvent); 
        }
    }
}
