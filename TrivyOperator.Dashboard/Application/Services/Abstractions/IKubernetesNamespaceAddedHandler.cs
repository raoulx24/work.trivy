namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesNamespaceAddedHandler
{
    Task Handle(string k8sNamespace);
}
