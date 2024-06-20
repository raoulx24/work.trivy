using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

//TODO: change name
public class KubernetesNamespaceAddedHandler(
    IConcurrentCache<string, List<VulnerabilityReportCR>> cache,
    IVulnerabilityReportDomainService domainService,
    ILogger<KubernetesHostedService> logger)
    : IKubernetesNamespaceAddedHandler
{
    public async Task Handle(string k8sNamespace)
    {
        List<VulnerabilityReportCR> vulnerabilityReportCrList =
            await domainService.GetTrivyVulnerabilities(k8sNamespace);
        //cache.TryAdd(k8sNamespace, vulnerabilityReportCrList);
        cache[k8sNamespace] = vulnerabilityReportCrList;

        logger.LogInformation("VulnerabilityReport Cache updated for namespace {k8sNamespace}", k8sNamespace);

    }
}
