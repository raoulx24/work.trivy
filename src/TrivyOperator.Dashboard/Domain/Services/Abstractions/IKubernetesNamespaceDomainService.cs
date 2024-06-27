namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;

public interface IKubernetesNamespaceDomainService
{
    Task<List<string>> GetKubernetesNamespaces();
    bool IsStaticList { get; }
}
