using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public abstract class
    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>(
        IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        IServiceProvider serviceProvider,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>>
            logger) : IKubernetesWatcher<TKubernetesObject> where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TKubernetesObjectList : IItems<TKubernetesObject>
    where TKubernetesWatcherEvent : IWatcherEvent<TKubernetesObject>, new()
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>

{
    protected readonly TBackgroundQueue BackgroundQueue = backgroundQueue;
    protected readonly Kubernetes KubernetesClient = kubernetesClientFactory.GetClient();

    protected readonly Dictionary<string, TaskWithCts> Watchers = [];

    public Task Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        string watcherKey = GetNamespaceFromSourceEvent(sourceKubernetesObject);
        logger.LogInformation(
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

        return Task.CompletedTask;
    }

    protected async Task CreateWatch(
        IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
        CancellationToken cancellationToken)
    {
        bool isRecoveringFromError = false;
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
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    if (isRecoveringFromError || isFreshStart)
                    {
                        using IServiceScope scope = serviceProvider.CreateScope();
                        IWatcherState watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                        await watcherState.ProcessWatcherSuccess(typeof(TKubernetesObject), watcherKey);
                        isRecoveringFromError = false;
                        isFreshStart = false;
                    }
                    logger.LogDebug("Sending to Queue - {kubernetesObjectType} - {kubernetesWatchEvent} - {watcherKey} - {kubernetesObjectName}",
                        typeof(TKubernetesObject).Name,
                        type.ToString(),
                        watcherKey,
                        item.Metadata.Name);

                    TKubernetesWatcherEvent kubernetesWatcherEvent =
                        new() { KubernetesObject = item, WatcherEventType = type };
                    await BackgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);
                }
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode is HttpStatusCode.Unauthorized
                                                         or HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IWatcherState watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                await watcherState.ProcessWatcherError(typeof(TKubernetesObject), watcherKey, hoe);
            }
            catch (TaskCanceledException)
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IWatcherState watcherState = scope.ServiceProvider.GetRequiredService<IWatcherState>();
                await watcherState.ProcessWatcherCancel(typeof(TKubernetesObject), watcherKey);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Watcher for {kubernetesObjectType} and key {watcherKey} crashed - {ex.Message}",
                    typeof(TKubernetesObject).Name,
                    watcherKey,
                    ex.Message);
            }

            bool enqueueErrorEventIsSuccessful = false;
            while (!cancellationToken.IsCancellationRequested && !enqueueErrorEventIsSuccessful)
            {
                try
                {
                    logger.LogDebug("Sending to Queue - {kubernetesObjectType} - EventWithError - {watcherKey} - {kubernetesObjectName}",
                        typeof(TKubernetesObject).Name,
                        watcherKey,
                        sourceKubernetesObject?.Metadata.Name);

                    await EnqueueWatcherEventWithError(sourceKubernetesObject);
                    enqueueErrorEventIsSuccessful = true;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Watcher for {kubernetesObjectType} and key {watcherKey} could not enqueue EventWithError - {ex.Message}",
                        typeof(TKubernetesObject).Name,
                        watcherKey,
                        ex.Message);
                    await Task.Delay(10000, cancellationToken);
                }
                
            }

            isRecoveringFromError = true;
        }
    }

    protected string GetNamespaceFromSourceEvent(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject) =>
        sourceKubernetesObject is V1Namespace
            ? sourceKubernetesObject.Metadata.Name
            : VarUtils.GetCacheRefreshKey(sourceKubernetesObject);

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(
        IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
        CancellationToken cancellationToken);

    protected abstract Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject);
}
