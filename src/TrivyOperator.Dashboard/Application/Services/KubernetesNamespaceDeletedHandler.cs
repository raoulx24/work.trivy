using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesNamespaceDeletedHandler(
    IConcurrentCache<string, List<VulnerabilityReportCR>> cache,
    ILogger<KubernetesNamespaceDeletedHandler> logger) : IKubernetesNamespaceDeletedHandler
{
    public Task Handle(string k8sNamespace)
    {
        cache.TryRemove(k8sNamespace, out _);
        logger.LogInformation("VulnerabilityReport Cache item deleted for namespace {k8sNamespace}", k8sNamespace);
        return Task.CompletedTask;
    }
}
