using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;

public interface ICacheWatcherEventHandler
{
    void Start(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}
