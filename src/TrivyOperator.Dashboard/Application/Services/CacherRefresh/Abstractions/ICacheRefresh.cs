
using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

public interface ICacheRefresh<TKubernetesObject, TBackgroundQueue>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TBackgroundQueue : IBackgroundQueue<TKubernetesObject>
{
    void StartEventsProcessing(CancellationToken cancellationToken);
    bool IsQueueProcessingStarted();
}