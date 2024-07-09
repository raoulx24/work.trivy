using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

public class CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue> : 
    BackgroundService, ICacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
        where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    protected TBackgroundQueue backgroundQueue { get; init; }
    protected IConcurrentCache<string, IList<TKubernetesObject>> cache { get; init; }
    protected ILogger<CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>> logger { get; init; }

    public CacheRefresh(TBackgroundQueue backgroundQueue,
        IConcurrentCache<string, IList<TKubernetesObject>> cache,
        ILogger<CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>> logger)
    {
        this.backgroundQueue = backgroundQueue;
        this.cache = cache;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Cache Refresh {nameof(CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>)} service running.");

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessChannelMessage(cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }

    protected async Task ProcessChannelMessage(CancellationToken cancellationToken)
    {
        TKubernetesWatcherEvent watcherEvent = await backgroundQueue.DequeueAsync(cancellationToken);
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

    protected void ProcessAddEvent(TKubernetesWatcherEvent watcherEvent, CancellationToken cancellationToken)
    {
        string eventNamespaceName = VarUtils.GetWatchersKey(watcherEvent.KubernetesObject);
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

    protected void ProcessDeleteEvent(TKubernetesWatcherEvent watcherEvent)
    {
        string eventNamespaceName = VarUtils.GetWatchersKey(watcherEvent.KubernetesObject);
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

    protected void ProcessErrorEvent(TKubernetesWatcherEvent watcherEvent)
    {
        string eventNamespaceName = VarUtils.GetWatchersKey(watcherEvent.KubernetesObject);
        // TODO Clarify cache[key] vs cache.Remove and cache.Add
        cache.TryRemove(eventNamespaceName, out _);
        cache.TryAdd(eventNamespaceName, new List<TKubernetesObject>());
    }
}
