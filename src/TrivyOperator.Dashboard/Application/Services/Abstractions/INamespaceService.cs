namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface INamespaceService
{
    Task<List<string>> GetKubernetesNamespaces();
}
