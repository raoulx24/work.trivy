using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class KubernetesNamespaceDomainService(
    IKubernetesClientFactory kubernetesClientFactory,
    ILogger<KubernetesNamespaceDomainService> logger) : IKubernetesNamespaceDomainService
{
    private readonly Kubernetes kubernetesClient = kubernetesClientFactory.GetClient();

    public async Task<List<string>> GetKubernetesNamespaces()
    {
        try
        {
            V1NamespaceList namespaceList = await kubernetesClient.CoreV1.ListNamespaceAsync();
            List<string> namespaceNames = [];
            namespaceNames.AddRange(namespaceList.Items.Select(item => item.Name()));
            return namespaceNames;
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.Forbidden)
        {
            logger.LogWarning(
                ex,
                "Cannot get Kubernetes Namespaces. Forbidden (403). Error: {responseContent}",
                ex.Response.Content);
            return [""];
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Cannot get Kubernetes Namespaces. Error {exceptionMessage}", ex.Message);
            throw;
        }
    }
}
