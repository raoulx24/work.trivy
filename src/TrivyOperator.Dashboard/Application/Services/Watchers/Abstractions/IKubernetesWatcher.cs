using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public interface IKubernetesWatcher<TKubernetesObject> where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
{
    Task Add(CancellationToken cancellationToken, IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject = null);
}
