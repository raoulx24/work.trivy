using k8s;

namespace TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

public interface IWatcherEvent<TKubernetesObject> where TKubernetesObject : IKubernetesObject
{
    WatchEventType WatcherEventType { get; init; }
    TKubernetesObject KubernetesObject { get; init; }
}
