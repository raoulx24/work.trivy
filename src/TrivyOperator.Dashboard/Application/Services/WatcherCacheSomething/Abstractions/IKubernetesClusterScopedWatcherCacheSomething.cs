using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
public interface IKubernetesClusterScopedWatcherCacheSomething
{
    void StartSomething(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}