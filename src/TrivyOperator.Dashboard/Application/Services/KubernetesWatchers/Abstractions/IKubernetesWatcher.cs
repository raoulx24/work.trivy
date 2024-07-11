﻿using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public interface IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>
    where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>
    where TSourceKubernetesObject : class, IKubernetesObject<V1ObjectMeta>
    where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>

{
    void Add(CancellationToken cancellationToken, TSourceKubernetesObject? sourceKubernetesObject = null);
}