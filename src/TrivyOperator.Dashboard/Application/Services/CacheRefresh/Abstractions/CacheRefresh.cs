using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;

public class CacheRefresh<TKubernetesObject, TBackgroundQueue>(
    TBackgroundQueue backgroundQueue,
    IConcurrentCache<string, IList<TKubernetesObject>> cache,
    ILogger<CacheRefresh<TKubernetesObject, TBackgroundQueue>> logger)
    : ICacheRefresh<TKubernetesObject, TBackgroundQueue> where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    protected TBackgroundQueue backgroundQueue = backgroundQueue;
    protected Task? CacheRefreshTask;

    public void StartEventsProcessing(CancellationToken cancellationToken)
    {
        if (CacheRefreshTask is not null)
        {
            logger.LogWarning(
                "Processing for {kubernetesObjectType} already started. Ignoring...",
                typeof(TKubernetesObject).Name);
            return;
        }

        logger.LogInformation("CacheRefresh for {kubernetesObjectType} is starting.", typeof(TKubernetesObject).Name);
        CacheRefreshTask = ProcessChannelMessages(cancellationToken);
    }

    public bool IsQueueProcessingStarted() => CacheRefreshTask is not null; // TODO: check for other task states

    protected virtual async Task ProcessChannelMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            IWatcherEvent<TKubernetesObject> watcherEvent = await backgroundQueue.DequeueAsync(cancellationToken);
            switch (watcherEvent.WatcherEventType)
            {
                case WatchEventType.Added:
                    ProcessAddEvent(watcherEvent, cancellationToken);
                    break;
                case WatchEventType.Deleted:
                    ProcessDeleteEvent(watcherEvent);
                    break;
                case WatchEventType.Error:
                    ProcessErrorEvent(watcherEvent);
                    break;
                case WatchEventType.Modified:
                    ProcessModifiedEvent(watcherEvent, cancellationToken);
                    break;
                    //default:
                    //    break;
            }
        }
    }

    protected virtual void ProcessAddEvent(
        IWatcherEvent<TKubernetesObject> watcherEvent,
        CancellationToken cancellationToken)
    {
        string watcherKey = VarUtils.GetCacheRefreshKey(watcherEvent.KubernetesObject);

        logger.LogDebug(
            "ProcessAddEvent - {kubernetesObjectType} - {watcherKey} - {kubernetesObjectName}",
            typeof(TKubernetesObject).Name,
            watcherKey,
            watcherEvent.KubernetesObject.Metadata.Name);

        if (cache.TryGetValue(watcherKey, out IList<TKubernetesObject>? kubernetesObjects))
        {
            IEnumerable<TKubernetesObject> existingKubernetesObjects =
                kubernetesObjects.Where(x => x.Uid == watcherEvent.KubernetesObject.Uid) ?? [];
            foreach (TKubernetesObject existingKubernetesObject in existingKubernetesObjects)
            {
                kubernetesObjects.Remove(existingKubernetesObject);
            }

            kubernetesObjects.Add(watcherEvent.KubernetesObject);
        }
        else // first time, the cache is really empty
        {
            cache.TryAdd(watcherKey, [watcherEvent.KubernetesObject]);
        }
    }

    protected virtual void ProcessDeleteEvent(IWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string watcherKey = VarUtils.GetCacheRefreshKey(watcherEvent.KubernetesObject);

        logger.LogDebug(
            "ProcessDeleteEvent - {kubernetesObjectType} - {watcherKey} - {kubernetesObjectName}",
            typeof(TKubernetesObject).Name,
            watcherKey,
            watcherEvent.KubernetesObject.Metadata.Name);

        if (cache.TryGetValue(watcherKey, out IList<TKubernetesObject>? kubernetesObjects))
        {
            IEnumerable<TKubernetesObject> existingKubernetesObjects =
                kubernetesObjects.Where(x => x.Uid == watcherEvent.KubernetesObject.Uid) ?? [];
            foreach (TKubernetesObject existingKubernetesObject in existingKubernetesObjects)
            {
                kubernetesObjects.Remove(existingKubernetesObject);
            }
        }
    }

    protected virtual void ProcessErrorEvent(IWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string watcherKey = VarUtils.GetCacheRefreshKey(watcherEvent.KubernetesObject);
        logger.LogDebug(
            "ProcessErrorEvent - {kubernetesObjectType} - {watcherKey}",
            typeof(TKubernetesObject).Name,
            watcherKey);
        cache.TryRemove(watcherKey, out _);
    }

    protected virtual void ProcessModifiedEvent(
        IWatcherEvent<TKubernetesObject> watcherEvent,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("ProcessModifiedEvent - redirecting to ProcessAddEvent.");
        ProcessAddEvent(watcherEvent, cancellationToken);
    }
}
