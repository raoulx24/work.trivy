using k8s;
using k8s.Models;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class KubernetesNamespaceDomainService(IK8sClientFactory k8sClientFactory) : IKubernetesNamespaceDomainService
{
    private readonly Kubernetes k8sClient = k8sClientFactory.GetClient();

    public async Task<List<string>> GetKubernetesNamespaces()
    {
        V1NamespaceList namespaceList = await k8sClient.CoreV1.ListNamespaceAsync();
        List<string> namespaceNames = [];
        namespaceNames.AddRange(namespaceList.Items.Select(item => item.Name()));
        return namespaceNames;
    }
}
