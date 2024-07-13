using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

public class CacheRefresh<TKubernetesObject, TBackgroundQueue> : 
    ICacheRefresh<TKubernetesObject, TBackgroundQueue>
        where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
        where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    protected TBackgroundQueue backgroundQueue { get; init; }
    protected IConcurrentCache<string, IList<TKubernetesObject>> cache { get; init; }
    protected ILogger<CacheRefresh<TKubernetesObject, TBackgroundQueue>> logger { get; init; }
    protected Task? cacheRefreshTask { get; set; }

    public CacheRefresh(TBackgroundQueue backgroundQueue,
        IConcurrentCache<string, IList<TKubernetesObject>> cache,
        ILogger<CacheRefresh<TKubernetesObject, TBackgroundQueue>> logger)
    {
        this.backgroundQueue = backgroundQueue;
        this.cache = cache;
        this.logger = logger;
    }

    public void StartEventsProcessing(CancellationToken cancellationToken)
    {
        if (cacheRefreshTask is not null)
        {
            logger.LogWarning("Processing for {kubernetesObjectType} already started. Ignoring...", typeof(TKubernetesObject).Name);
            return;
        }

        logger.LogInformation("CacheRefresh for {kubernetesObjectType} is starting.", typeof(TKubernetesObject).Name);
        cacheRefreshTask = ProcessChannelMessages(cancellationToken);
    }

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
                    //default:
                    //    break;
            }
        }
    }

    protected virtual void ProcessAddEvent(IWatcherEvent<TKubernetesObject> watcherEvent, CancellationToken cancellationToken)
    {
        string watcherKey = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        string eventKubernetesObjectName = watcherEvent.KubernetesObject.Metadata.Name;

        logger.LogDebug("ProcessAddEvent - {kubernetesObjectType} - {watcherKey} - {kubernetesObjectName}",
            typeof(TKubernetesObject).Name, watcherKey, eventKubernetesObjectName);

        if (cache.TryGetValue(watcherKey, value: out IList<TKubernetesObject>? kubernetesObjects))
        {
            // TODO try catch - clear duplicates
            TKubernetesObject? potentialExistingKubernetesObject = kubernetesObjects.SingleOrDefault(x => x.Metadata.Name == eventKubernetesObjectName);
            if (potentialExistingKubernetesObject is not null)
            {
                kubernetesObjects.Remove(potentialExistingKubernetesObject);
            }
            kubernetesObjects.Add(watcherEvent.KubernetesObject);
            // TODO Clarify cache[key] vs cache.Remove and cache.Add
            cache[watcherKey] = kubernetesObjects;
        }
        else // first time, the cache is really empty
        {
            cache.TryAdd(watcherKey, new List<TKubernetesObject>() { watcherEvent.KubernetesObject });
        }
    }

    protected virtual void ProcessDeleteEvent(IWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string watcherKey = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        string eventKubernetesObjectName = watcherEvent.KubernetesObject.Metadata.Name;

        logger.LogDebug("ProcessAddEvent - {kubernetesObjectType} - {watcherKey} - {kubernetesObjectName}",
            typeof(TKubernetesObject).Name, watcherKey, eventKubernetesObjectName);

        if (cache.TryGetValue(watcherKey, value: out IList<TKubernetesObject>? kubernetesObjects))
        {
            // TODO try catch - clear duplicates
            TKubernetesObject? toBedeletedKubernetesObject = kubernetesObjects.SingleOrDefault(x => x.Metadata.Name == eventKubernetesObjectName);
            if (toBedeletedKubernetesObject is not null)
            {
                kubernetesObjects.Remove(toBedeletedKubernetesObject);
            }
            // TODO Clarify cache[key] vs cache.Remove and cache.Add
            cache.TryRemove(watcherKey, out _);
            cache.TryAdd(watcherKey, kubernetesObjects);
        }
    }

    protected virtual void ProcessErrorEvent(IWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string watcherKey = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        logger.LogDebug("ProcessAddEvent - {kubernetesObjectType} - {watcherKey}",
            typeof(TKubernetesObject).Name, watcherKey);
        // TODO Clarify cache[key] vs cache.Remove and cache.Add
        cache.TryRemove(watcherKey, out _);
        cache.TryAdd(watcherKey, new List<TKubernetesObject>());
    }

    public bool IsQueueProcessingStarted()
    {
        // TODO: check for other task states
        return (cacheRefreshTask is not null);
    }
}
