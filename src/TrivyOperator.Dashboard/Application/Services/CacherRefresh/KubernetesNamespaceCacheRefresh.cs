using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;
using TrivyOperator.Dashboard.Application.Services.WatcherParams;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh;

public class KubernetesNamespaceCacheRefresh : CacheRefresh<V1Namespace, KubernetesNamespaceWatcherEvent, KubernetesNamespaceBackgroundQueue>
{
    public KubernetesNamespaceCacheRefresh(KubernetesNamespaceBackgroundQueue backgroundQueue,
        IConcurrentCache<string, List<V1Namespace>> cache,
        ILogger<CacheRefresh<V1Namespace, KubernetesNamespaceWatcherEvent, KubernetesNamespaceBackgroundQueue>> logger) 
        : base(backgroundQueue, cache, logger)
    { }

    protected override void ProcessAddEvent(KubernetesNamespaceWatcherEvent watcherEvent, CancellationToken cancellationToken)
    {
        string eventNamespaceName = watcherEvent.KubernetesObject.Metadata.Name;
        
        if (cache.TryGetValue("genericKey", value: out List<V1Namespace>? kubernetesNamespaces))
        {
            // TODO try catch - clear duplicates
            V1Namespace potentialExistingKubernetesObject = kubernetesNamespaces.SingleOrDefault(x => x.Metadata.Name == eventNamespaceName) ?? null;
            if (potentialExistingKubernetesObject is not null)
            {
                kubernetesNamespaces.Remove(potentialExistingKubernetesObject);
            }
            kubernetesNamespaces.Add(watcherEvent.KubernetesObject);
        }
        else // first time, the cache is really empty
        {
            cache.TryAdd("genericKey", new() { watcherEvent.KubernetesObject });
        }
        // TODO foreach singletons of type IKubernetesNamespacedWatcher, call Add
        //KubernetesNamespacedWatcherParams watcherParams = new() 
        //    { CancellationToken = cancellationToken, kubernetesNamespace = eventNamespaceName };
    }

    protected override void ProcessDeleteEvent(KubernetesNamespaceWatcherEvent watcherEvent)
    {
        string eventNamespaceName = watcherEvent.KubernetesObject.Metadata.Name;

        if (cache.TryGetValue("genericKey", value: out List<V1Namespace>? kubernetesNamespaces))
        {
            // TODO try catch - clear duplicates
            V1Namespace toBedeletedV1Namespace = kubernetesNamespaces.SingleOrDefault(x => x.Metadata.Name == eventNamespaceName) ?? null;
            if (toBedeletedV1Namespace is not null)
            {
                kubernetesNamespaces.Remove(toBedeletedV1Namespace);
            }
        }
        // TODO foreach singletons of type IKubernetesNamespacedWatcher, call Delete
        //KubernetesNamespacedWatcherParams watcherParams = new()
        //{ CancellationToken = new(), kubernetesNamespace = eventNamespaceName };
    }

    protected override void ProcessErrorEvent(KubernetesNamespaceWatcherEvent watcherEvent)
    {
        cache.TryRemove("genericKey", out _);
        cache.TryAdd("genericKey", new());
    }
}
