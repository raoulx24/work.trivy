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
            logger.LogWarning("Processing already started. Ignoring...");
            return;
        }
        
        cacheRefreshTask = ProcessChannelMessages(cancellationToken);
    }

    protected virtual async Task ProcessChannelMessages(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            IKubernetesWatcherEvent<TKubernetesObject> watcherEvent = await backgroundQueue.DequeueAsync(cancellationToken);
            switch (watcherEvent.WatcherEvent)
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

    protected virtual void ProcessAddEvent(IKubernetesWatcherEvent<TKubernetesObject> watcherEvent, CancellationToken cancellationToken)
    {
        string eventNamespaceName = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        string eventKubernetesObjectName = watcherEvent.KubernetesObject.Metadata.Name;

        if (cache.TryGetValue(eventNamespaceName, value: out IList<TKubernetesObject>? kubernetesObjects))
        {
            // TODO try catch - clear duplicates
            TKubernetesObject? potentialExistingKubernetesObject = kubernetesObjects.SingleOrDefault(x => x.Metadata.Name == eventKubernetesObjectName);
            if (potentialExistingKubernetesObject is not null)
            {
                kubernetesObjects.Remove(potentialExistingKubernetesObject);
            }
            kubernetesObjects.Add(watcherEvent.KubernetesObject);
            // TODO Clarify cache[key] vs cache.Remove and cache.Add
            cache[eventNamespaceName] = kubernetesObjects;
        }
        else // first time, the cache is really empty
        {
            cache.TryAdd(eventNamespaceName, new List<TKubernetesObject>() { watcherEvent.KubernetesObject });
        }
    }

    protected virtual void ProcessDeleteEvent(IKubernetesWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string eventNamespaceName = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        string eventKubernetesObjectName = watcherEvent.KubernetesObject.Metadata.Name;

        if (cache.TryGetValue(eventNamespaceName, value: out IList<TKubernetesObject>? kubernetesObjects))
        {
            // TODO try catch - clear duplicates
            TKubernetesObject? toBedeletedKubernetesObject = kubernetesObjects.SingleOrDefault(x => x.Metadata.Name == eventKubernetesObjectName);
            if (toBedeletedKubernetesObject is not null)
            {
                kubernetesObjects.Remove(toBedeletedKubernetesObject);
            }
            // TODO Clarify cache[key] vs cache.Remove and cache.Add
            cache.TryRemove(eventNamespaceName, out _);
            cache.TryAdd(eventNamespaceName, kubernetesObjects);
        }
    }

    protected virtual void ProcessErrorEvent(IKubernetesWatcherEvent<TKubernetesObject> watcherEvent)
    {
        string eventNamespaceName = VarUtils.GetCacherRefreshKey(watcherEvent.KubernetesObject);
        // TODO Clarify cache[key] vs cache.Remove and cache.Add
        cache.TryRemove(eventNamespaceName, out _);
        cache.TryAdd(eventNamespaceName, new List<TKubernetesObject>());
    }

    public bool IsQueueProcessingStarted()
    {
        // TODO: check for other task states
        return (cacheRefreshTask is not null);
    }
}
