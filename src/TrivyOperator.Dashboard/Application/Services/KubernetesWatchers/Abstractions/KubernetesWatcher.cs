using k8s;
using k8s.Autorest;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent> :
    IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesWatcherParams
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
        where TBackgoundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
        
{
    protected Kubernetes kubernetesClient;
    protected IBackgroundQueue<IKubernetesWatcherEvent<TKubernetesObject>, TKubernetesObject> backgroundQueue;
    protected ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>> logger;
    protected Dictionary<string, TaskWithCts> watchers = [];

    public KubernetesWatcher(IKubernetesClientFactory kubernetesClientFactory,
        IBackgroundQueue<IKubernetesWatcherEvent<TKubernetesObject>, TKubernetesObject> backgroundQueue,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>> logger)
    {
        kubernetesClient = kubernetesClientFactory.GetClient();
        this.backgroundQueue = backgroundQueue;
        this.logger = logger;
    }

    public void Add(TWatcherParams watcherParams)
    {
        CancellationTokenSource cts = new();
        TaskWithCts taskWithCts = new()
        {
            Task = CreateWatch(
                        watcherParams,
                        CancellationTokenSource.CreateLinkedTokenSource(watcherParams.CancellationToken, cts.Token).Token),
            Cts = cts,
        };
        TaskWithCts watcherWithCts = taskWithCts;

        AddWatcherWithCtsToWatchers(watcherWithCts, watcherParams);
    }

    protected async Task CreateWatch(TWatcherParams watcherParams, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TKubernetesWatcherEvent kubernetesWatcherEvent = GetKubernetesWatcherEventWithError(watcherParams);
            await backgroundQueue.QueueBackgroundWorkItemAsync(kubernetesWatcherEvent);

            try
            {
                Task<HttpOperationResponse<TKubernetesObjectList>> listNamespaceResp = GetKubernetesObjectWatchList(watcherParams, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in listNamespaceResp
                                   .WatchAsync<TKubernetesObject, TKubernetesObjectList>(
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    kubernetesWatcherEvent = new() { KubernetesObject = item, WatcherEvent = type };
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
                logger.LogError($"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgoundQueue, TKubernetesWatcherEvent>)} - {ex.Message}", ex);
            }
        }
    }

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(TWatcherParams watcherParams, CancellationToken cancellationToken);
    protected abstract void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams);
    protected abstract TKubernetesWatcherEvent GetKubernetesWatcherEventWithError(TWatcherParams watcherParams);
}
