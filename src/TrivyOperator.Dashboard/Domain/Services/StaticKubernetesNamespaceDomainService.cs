using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class StaticKubernetesNamespaceDomainService(IConfiguration configuration, ILogger<KubernetesHostedService> logger) : IKubernetesNamespaceDomainService
{
    public bool IsStaticList => true;

    public async Task<List<string>> GetKubernetesNamespaces()
    {
        string configK8sNamespaces = configuration.GetSection("Kubernetes").GetValue<string>("NamespaceList");

        if (string.IsNullOrWhiteSpace(configK8sNamespaces))
        {
            throw new ArgumentNullException("Provided parameter Kubernetes.NamespaceList is null or whitespace.");
        }

        List<string> k8sNamespaces = configK8sNamespaces.Split(',').Select(x => x.Trim()).ToList();

        return k8sNamespaces;
    }
}
