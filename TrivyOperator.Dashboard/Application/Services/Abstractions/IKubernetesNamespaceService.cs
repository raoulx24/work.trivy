namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IKubernetesNamespaceService
{
    Task<List<string>> GetKubernetesNamespaces();
}
