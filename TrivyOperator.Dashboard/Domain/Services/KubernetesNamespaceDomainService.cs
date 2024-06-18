namespace TrivyOperator.Dashboard.Domain.Services
{
    using k8s;
    using k8s.Models;
    using TrivyOperator.Dashboard.Domain.Services.Abstractions;
    using TrivyOperator.Dashboard.Infrastructure.Abstractions;

    public class KubernetesNamespaceDomainService : IKubernetesNamespaceDomainService
    {
        private Kubernetes k8sClient { get; set; }

        public KubernetesNamespaceDomainService(IK8sClientFactory k8sClientFactory)
        {
            k8sClient = k8sClientFactory.GetClient();
        }

        public async Task<List<string>> GetKubernetesNamespaces()
        {
            V1NamespaceList namespaceList = await k8sClient.CoreV1.ListNamespaceAsync();

            List<string> namespaceNames = new();

            foreach (V1Namespace v1Namespace in namespaceList.Items)
            {
                namespaceNames.Add(v1Namespace.Metadata.Name);
            }

            return namespaceNames;
        }
    }
}
