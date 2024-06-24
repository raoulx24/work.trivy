namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesNamespaceDeletedHandler
{
    Task Handle(string k8sNamespace);
}
