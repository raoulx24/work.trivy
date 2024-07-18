using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;

public interface INamespacedCacheWatcherEventHandler : ICacheWatcherEventHandler
{
    void Stop(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}
