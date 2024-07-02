using k8s;

namespace TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
public interface IKubernetesWatcherEvent<TKubernetesObject> where TKubernetesObject : IKubernetesObject
{
    TKubernetesObject KubernetesObject { get; init; }
    WatchEventType WatcherEvent { get; init; }
}