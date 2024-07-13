using k8s.Models;
using k8s;

namespace TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;

public interface IClusterScopedWatcher<TKubernetesObject> : IKubernetesWatcher<TKubernetesObject>
    where TKubernetesObject : IKubernetesObject<V1ObjectMeta>
{ }