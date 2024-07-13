using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    IKubernetesWatcher<TKubernetesObject>
        where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
        where TKubernetesObjectList : IItems<TKubernetesObject>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
        
{
    protected Kubernetes kubernetesClient;
    protected TBackgroundQueue backgroundQueue;
    protected ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger;
    protected Dictionary<string, TaskWithCts> watchers = [];

    public KubernetesWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
    {
        kubernetesClient = kubernetesClientFactory.GetClient();
        this.backgroundQueue = backgroundQueue;
        this.logger = logger;
    }

    public void Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null)
    {
        string watcherKey = GetNamespaceFromSourceEvent(sourceKubernetesObject);
        logger.LogInformation("Adding Watcher for {kubernetesObjectType} and key {watcherKey}.", typeof(TKubernetesObject), watcherKey);
        CancellationTokenSource cts = new();
        TaskWithCts watcherWithCts = new()
        {
            Task = CreateWatch(
                        sourceKubernetesObject,
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token).Token),
            Cts = cts,
        };

        watchers.Add(watcherKey, watcherWithCts);
    }

    protected async Task CreateWatch(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string watcherKey = GetNamespaceFromSourceEvent(sourceKubernetesObject);
            try
            {
                Task<HttpOperationResponse<TKubernetesObjectList>> kubernetesObjectsResp = GetKubernetesObjectWatchList(sourceKubernetesObject, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in kubernetesObjectsResp
                                   .WatchAsync<TKubernetesObject, TKubernetesObjectList>(
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    TKubernetesWatcherEvent kubernetesWatcherEvent = new() { KubernetesObject = item, WatcherEvent = type };
                    await backgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);
                }
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogError("Watcher for {kubernetesObjectType} and key {watcherKey} crashed with 404.", typeof(TKubernetesObject), watcherKey);
                // TODO: something something
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.Forbidden)
            {
                logger.LogError("Watcher for {kubernetesObjectType} and key {watcherKey} crashed with 403.", typeof(TKubernetesObject), watcherKey);
                // TODO: something something
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Watcher for {kubernetesObjectType} and key {watcherKey} crashed - {ex.Message}", typeof(TKubernetesObject), watcherKey, ex.Message);
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                await EnqueueWatcherEventWithError(sourceKubernetesObject);
            }
        }
    }

    protected string GetNamespaceFromSourceEvent(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject)
    {
        if (sourceKubernetesObject != null && sourceKubernetesObject is V1Namespace)
        {
            return sourceKubernetesObject.Metadata.Name;
        }
        
        return VarUtils.GetCacherRefreshKey(sourceKubernetesObject);
    }

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject, CancellationToken cancellationToken);
    protected abstract Task EnqueueWatcherEventWithError(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject);
}
