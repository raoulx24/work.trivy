//using k8s;
//using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
//using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
//using TrivyOperator.Dashboard.Infrastructure.Abstractions;

//namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

//public abstract class KubernetesClusterScopedCacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue> :
//    CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
//        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
//        where TKubernetesObject : IKubernetesObject
//        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
//{
//    protected KubernetesClusterScopedCacheRefresh(TBackgroundQueue backgroundQueue, 
//        IConcurrentCache<string, List<TKubernetesObject>> cache, 
//        ILogger<CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>> logger) 
//        : base(backgroundQueue, cache, logger)
//    { }
//}
