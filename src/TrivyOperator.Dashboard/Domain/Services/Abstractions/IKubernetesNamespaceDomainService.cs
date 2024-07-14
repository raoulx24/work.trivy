namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;

public interface IKubernetesNamespaceDomainService
{
    bool IsStaticList { get; }
    Task<List<string>> GetKubernetesNamespaces();
}
