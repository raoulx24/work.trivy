using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class KubernetesNamespaceDomainService(IK8sClientFactory k8sClientFactory, ILogger<KubernetesHostedService> logger) : IKubernetesNamespaceDomainService
{
    private readonly Kubernetes k8sClient = k8sClientFactory.GetClient();

    public bool IsStaticList => false;

    public async Task<List<string>> GetKubernetesNamespaces()
    {
        try
        {
            V1NamespaceList namespaceList = await k8sClient.CoreV1.ListNamespaceAsync();
            List<string> namespaceNames = [];
            namespaceNames.AddRange(namespaceList.Items.Select(item => item.Name()));
            return namespaceNames;
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            logger.LogWarning($"Cannot get Kubernetes Namespaces. Forbidden (403). Error: {ex.Response.Content}");
            return new List<string> { "" };
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Cannot get Kubernetes Namespaces. Error {ex.Message}");
            throw;
        }
    }
}
