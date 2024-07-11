﻿using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers;

public class StaticNamespaceWatcher : IKubernetesWatcher<V1NamespaceList, V1Namespace, IKubernetesObject<V1ObjectMeta>, BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>, KubernetesWatcherEvent<V1Namespace>>,
    IKubernetesClusterScopedWatcher
{
    private BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace> backgroundQueue;
    private IKubernetesNamespaceDomainService kubernetesNamespaceDomainService;
    private ILogger<StaticNamespaceWatcher> logger;

    public StaticNamespaceWatcher(BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace> backgroundQueue,
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
            await backgroundQueue.QueueBackgroundWorkItemAsync(new() { KubernetesObject = v1Namespace, WatcherEvent = WatchEventType.Added }); 
        }
    }
}
