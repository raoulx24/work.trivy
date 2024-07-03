using k8s;

namespace TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
public interface IKubernetesWatcherEvent<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
{
    WatchEventType WatcherEvent { get; init; }
    TKubernetesObject? KubernetesObject { get; init; }
}