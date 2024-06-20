namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesNamespaceAddedOrModifiedHandler
{
    Task Handle(string k8sNamespace);
}
