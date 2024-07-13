using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public interface IKubernetesWatcher<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
{
    void Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}