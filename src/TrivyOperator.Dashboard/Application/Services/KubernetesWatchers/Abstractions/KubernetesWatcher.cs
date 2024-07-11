using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent> :
    IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : class, IKubernetesObject<V1ObjectMeta>, new()
        where TKubernetesObjectList : IItems<TKubernetesObject>
        where TSourceKubernetesObject: class, IKubernetesObject<V1ObjectMeta>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
        
{
    protected Kubernetes kubernetesClient;
    protected TBackgroundQueue backgroundQueue;
    protected ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger;
    protected Dictionary<string, TaskWithCts> watchers = [];

    public KubernetesWatcher(IKubernetesClientFactory kubernetesClientFactory,
        TBackgroundQueue backgroundQueue,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
    {
        kubernetesClient = kubernetesClientFactory.GetClient();
        this.backgroundQueue = backgroundQueue;
        this.logger = logger;
    }

    public void Add(CancellationToken cancellationToken, TSourceKubernetesObject? sourceKubernetesObject = null)
    {
        CancellationTokenSource cts = new();
        TaskWithCts watcherWithCts = new()
        {
            Task = CreateWatch(
                        sourceKubernetesObject,
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token).Token),
            Cts = cts,
        };

        watchers.Add(VarUtils.GetWatchersKey(sourceKubernetesObject), watcherWithCts);
    }

    protected async Task CreateWatch(TSourceKubernetesObject? sourceKubernetesObject, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await EnqueueWatcherEventWithError(sourceKubernetesObject);

            //TKubernetesWatcherEvent kubernetesWatcherEvent = GetKubernetesWatcherEventWithError(sourceKubernetesObject);
            //await backgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);

            try
            {
                Task<HttpOperationResponse<TKubernetesObjectList>> listNamespaceResp = GetKubernetesObjectWatchList(sourceKubernetesObject, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in listNamespaceResp
                                   .WatchAsync<TKubernetesObject, TKubernetesObjectList>(
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    TKubernetesWatcherEvent kubernetesWatcherEvent = new() { KubernetesObject = item, WatcherEvent = type };
                    await backgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);
                }
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // TODO: something something
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.Forbidden)
            {
                // TODO: something something
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TSourceKubernetesObject, TBackgroundQueue, TKubernetesWatcherEvent>)} - {ex.Message}", ex);
            }
        }
    }

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(TSourceKubernetesObject? sourceKubernetesObject, CancellationToken cancellationToken);
    protected abstract Task EnqueueWatcherEventWithError(TSourceKubernetesObject? sourceKubernetesObject);
}
