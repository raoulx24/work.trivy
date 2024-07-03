using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents;

namespace TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
public interface IKubernetesNamespaceBackgroundQueue :
    IBackgroundQueue<KubernetesNamespaceWatcherEvent, V1Namespace>
{
}