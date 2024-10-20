﻿using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        IServiceProvider serviceProvider,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
    : IKubernetesWatcher<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>
    where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>

{
    protected readonly TBackgroundQueue BackgroundQueue = backgroundQueue;
    protected readonly Kubernetes KubernetesClient = kubernetesClientFactory.GetClient();

    protected readonly ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue,
        TKubernetesWatcherEvent>> Logger = logger;

    protected readonly Dictionary<string, TaskWithCts> Watchers = [];

    public void Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        string watcherKey = GetNamespaceFromSourceEvent(sourceKubernetesObject);
        Logger.LogInformation(
            "Adding Watcher for {kubernetesObjectType} and key {watcherKey}.",
            typeof(TKubernetesObject).Name,
            watcherKey);
        CancellationTokenSource cts = new();
        TaskWithCts watcherWithCts = new()
        {
            Task = CreateWatch(
                sourceKubernetesObject,
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token).Token),
            Cts = cts,
        };

        Watchers.Add(watcherKey, watcherWithCts);
    }

    protected async Task CreateWatch(
        IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
        CancellationToken cancellationToken)
    {
        bool isRecoveringfromError = false;
        bool isFreshStart = true;
        while (!cancellationToken.IsCancellationRequested)
        {
            string watcherKey = GetNamespaceFromSourceEvent(sourceKubernetesObject);
            try
            {
                Task<HttpOperationResponse<TKubernetesObjectList>> kubernetesObjectsResp =
                    GetKubernetesObjectWatchList(sourceKubernetesObject, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in kubernetesObjectsResp
                                   .WatchAsync<TKubernetesObject, TKubernetesObjectList>(
                                       ex => Logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    if (isRecoveringfromError || isFreshStart)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                        await watcherState.ProcessWatcherSuccess(typeof(TKubernetesObject), watcherKey);
                        isRecoveringfromError = false;
                        isFreshStart = false;
                    }
                    TKubernetesWatcherEvent kubernetesWatcherEvent =
                        new() { KubernetesObject = item, WatcherEventType = type };
                    await BackgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);
                }
            }
            catch (HttpOperationException hoe) when (
                hoe.Response.StatusCode == HttpStatusCode.Unauthorized ||
                hoe.Response.StatusCode == HttpStatusCode.Forbidden ||
                hoe.Response.StatusCode == HttpStatusCode.NotFound
                )
            {
                using var scope = serviceProvider.CreateScope();
                var watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                await watcherState.ProcessWatcherError(typeof(TKubernetesObject), watcherKey, hoe);
            }
            catch (TaskCanceledException)
            {
                using var scope = serviceProvider.CreateScope();
                var watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                await watcherState.ProcessWatcherCancel(typeof(TKubernetesObject), watcherKey);
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Watcher for {kubernetesObjectType} and key {watcherKey} crashed - {ex.Message}",
                    typeof(TKubernetesObject).Name,
                    watcherKey,
                    ex.Message);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await EnqueueWatcherEventWithError(sourceKubernetesObject);
                isRecoveringfromError = true;
            }
        }
    }

    protected string GetNamespaceFromSourceEvent(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        if (sourceKubernetesObject is V1Namespace)
        {
            return sourceKubernetesObject.Metadata.Name;
        }

        return VarUtils.GetCacheRefreshKey(sourceKubernetesObject);
    }

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(
        IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
        CancellationToken cancellationToken);

    protected abstract Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject);
}
