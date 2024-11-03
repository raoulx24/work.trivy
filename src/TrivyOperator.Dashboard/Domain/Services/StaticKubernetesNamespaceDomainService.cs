using Microsoft.Extensions.Options;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class StaticKubernetesNamespaceDomainService(
    IOptions<KubernetesOptions> kubernetesOptions,
    ILogger<StaticKubernetesNamespaceDomainService> logger) : IKubernetesNamespaceDomainService
{
    public Task<List<string>> GetKubernetesNamespaces()
    {
        string? configKubernetesNamespaces = kubernetesOptions.Value.NamespaceList;

        if (string.IsNullOrWhiteSpace(configKubernetesNamespaces))
        {
            throw new ArgumentNullException(
                "Provided parameter Kubernetes.NamespaceList is null or whitespace.",
                (Exception?)null);
        }

        List<string> kubernetesNamespaces = configKubernetesNamespaces.Split(',').Select(x => x.Trim()).ToList();
        logger.LogDebug("Found {listCount} kubernetes namespace names.", kubernetesNamespaces.Count);

        return Task.FromResult(kubernetesNamespaces);
    }
}
