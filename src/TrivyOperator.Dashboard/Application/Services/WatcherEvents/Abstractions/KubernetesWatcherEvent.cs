using k8s;

namespace TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

public class KubernetesWatcherEvent<TKubernetesObject> : IKubernetesWatcherEvent<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
{
    public WatchEventType WatcherEvent { get; init; }
    public TKubernetesObject? KubernetesObject { get; init; }
}
