using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
public interface IKubernetesNamespacedWatcherCacheSomething : IWatcherCacheSomething
{
    void StopSomething(IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}