using k8s;

namespace TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

public class WatcherEvent<TKubernetesObject> : IWatcherEvent<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
{
    public WatchEventType WatcherEventType { get; init; }
    public TKubernetesObject KubernetesObject { get; init; } = default!;
}
