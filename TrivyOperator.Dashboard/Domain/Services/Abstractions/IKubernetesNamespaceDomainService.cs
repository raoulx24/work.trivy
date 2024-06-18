namespace TrivyOperator.Dashboard.Domain.Services.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IKubernetesNamespaceDomainService
    {
        Task<List<string>> GetKubernetesNamespaces();
    }
}
