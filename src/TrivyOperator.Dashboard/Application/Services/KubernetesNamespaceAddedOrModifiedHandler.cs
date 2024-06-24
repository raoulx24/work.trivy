using k8s.Autorest;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesNamespaceAddedOrModifiedHandler(
    IConcurrentCache<string, List<VulnerabilityReportCR>> cache,
    IVulnerabilityReportDomainService domainService,
    ILogger<KubernetesNamespaceAddedOrModifiedHandler> logger) : IKubernetesNamespaceAddedOrModifiedHandler
{
    public async Task Handle(string k8sNamespace)
    {
        try
        {
            cache[k8sNamespace] = await domainService.GetTrivyVulnerabilities(k8sNamespace);
            logger.LogInformation(
                "VulnerabilityReport Cache item added/updated for namespace {k8sNamespace}",
                k8sNamespace);
        }
        catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.NotFound)
        {
            // Ignore
        }
    }
}
