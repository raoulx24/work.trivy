using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesNamespaceService(IKubernetesNamespaceDomainService kubernetesNamespaceDomainService)
    : IKubernetesNamespaceService
{
    public async Task<List<string>> GetKubernetesNamespaces()
    {
        return await kubernetesNamespaceDomainService.GetKubernetesNamespaces();
    }
}
