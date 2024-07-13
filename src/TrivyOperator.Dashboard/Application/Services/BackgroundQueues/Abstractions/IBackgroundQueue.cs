using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
public interface IBackgroundQueue<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
{
    ValueTask<IKubernetesWatcherEvent<TKubernetesObject>> DequeueAsync(CancellationToken cancellationToken);
    ValueTask QueueBackgroundWorkItemAsync(IKubernetesWatcherEvent<TKubernetesObject> workItem);
}