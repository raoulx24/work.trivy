//using k8s;
//using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
//using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
//using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
//using TrivyOperator.Dashboard.Infrastructure.Abstractions;

//namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

//public abstract class KubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent> :
//    KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>,
//    IKubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>
//        where TKubernetesObject : IKubernetesObject
//        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
//        where TWatcherParams : IKubernetesClusterScopedWatcherParams
//        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>, new()
//        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
//{
//    public KubernetesClusterScopedWatcher(IKubernetesClientFactory kubernetesClientFactory,
//        TBackgroundQueue backgroundQueue,
//        ILogger<KubernetesClusterScopedWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams, TBackgroundQueue, TKubernetesWatcherEvent>> logger)
//        : base(kubernetesClientFactory, backgroundQueue, logger)
//    {
//    }

//    protected override void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams)
//    {
//        watchers.Add("genericKey", watcher);
//    }
//}
