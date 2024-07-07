
using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;

public interface ICacheRefresh<TKubernetesObject, TKubernetesWatcherEvent, TBackgroundQueue>
    where TKubernetesWatcherEvent : IKubernetesWatcherEvent<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
    where TBackgroundQueue : IBackgroundQueue<TKubernetesWatcherEvent, TKubernetesObject>
{

}