namespace TrivyOperator.Dashboard.Application.Services.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IKubernetesNamespaceService
    {
        Task<List<string>> GetKubenetesNamespaces();
    }
}