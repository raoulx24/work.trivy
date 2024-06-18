namespace TrivyOperator.Dashboard.Application.Services
{
    using TrivyOperator.Dashboard.Domain.Services.Abstractions;
    using TrivyOperator.Dashboard.Domain.Services;
    using k8s.Models;
    using TrivyOperator.Dashboard.Application.Services.Abstractions;

    public class KubernetesNamespaceService : IKubernetesNamespaceService
    {
        private IKubernetesNamespaceDomainService kubernetesNamespaceDomainService { get; set; }

        public KubernetesNamespaceService(IKubernetesNamespaceDomainService kubernetesNamespaceDomainService)
        {
            this.kubernetesNamespaceDomainService = kubernetesNamespaceDomainService;
        }

        public async Task<List<string>> GetKubernetesNamespaces()
        {
            return await kubernetesNamespaceDomainService.GetKubernetesNamespaces();
        }
    }
}
