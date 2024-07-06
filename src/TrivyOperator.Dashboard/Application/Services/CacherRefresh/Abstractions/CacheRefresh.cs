using k8s;
using System.Linq.Expressions;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Services;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

public abstract class CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
        where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
        where TKubernetesObject : IKubernetesObject
        where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{
    protected TBackgroundQueue backgroundQueue { get; init; }
    protected IConcurrentCache<string, List<TKubernetesObject>> cache { get; init; }
    protected ILogger<CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>> logger { get; init; }

    public CacheRefresh(TBackgroundQueue backgroundQueue,
        IConcurrentCache<string, List<TKubernetesObject>> cache,
        ILogger<CacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>> logger)
    {
        this.backgroundQueue = backgroundQueue;
        this.cache = cache;
        this.logger = logger;
    }

    public async Task ProcessChannelMessage(CancellationToken cancellationToken)
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
            //default:
            //    break;
        }

    }

    protected abstract void ProcessAddEvent(TKubernetesWatcherEvent watcherEvent, CancellationToken cancellationToken);
    protected abstract void ProcessDeleteEvent(TKubernetesWatcherEvent watcherEvent);
}
