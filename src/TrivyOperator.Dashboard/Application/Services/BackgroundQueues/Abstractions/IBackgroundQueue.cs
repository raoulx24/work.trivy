using k8s;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
public interface IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
    where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
{
    ValueTask<TKubernetesWatcherEvent> DequeueAsync(CancellationToken cancellationToken);
    ValueTask QueueBackgroundWorkItemAsync(TKubernetesWatcherEvent workItem);
}