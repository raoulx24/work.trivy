using k8s;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesWatcher<TK8sObjectList, TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
    where TK8sObjectList : IKubernetesObject, IItems<TKubernetesObject>

{
    Task Watch(Kubernetes k8sClient, CancellationToken cancellationToken, string? k8sNamespace);
}