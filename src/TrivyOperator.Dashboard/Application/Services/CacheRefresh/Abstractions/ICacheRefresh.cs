using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;

public interface ICacheRefresh<TKubernetesObject, TBackgroundQueue>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    void StartEventsProcessing(CancellationToken cancellationToken);
    bool IsQueueProcessingStarted();
}
