using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Clients;

namespace TrivyOperator.Dashboard.Domain.Services;

public class KubernetesNamespaceDomainService(IKubernetesClientFactory kubernetesClientFactory, ILogger<KubernetesHostedService> logger) : IKubernetesNamespaceDomainService
{
    private readonly Kubernetes kubernetesClient = kubernetesClientFactory.GetClient();

    public bool IsStaticList => false;

    public async Task<List<string>> GetKubernetesNamespaces()
    {
        try
        {
            V1NamespaceList namespaceList = await kubernetesClient.CoreV1.ListNamespaceAsync();
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
