using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues;

public class KubernetesNamespaceBackgroundQueue :
    BackgroundQueue<KubernetesNamespaceWatcherEvent, V1Namespace>, IKubernetesNamespaceBackgroundQueue
{
    public KubernetesNamespaceBackgroundQueue(int capacity) : base(capacity)
    { }
}
