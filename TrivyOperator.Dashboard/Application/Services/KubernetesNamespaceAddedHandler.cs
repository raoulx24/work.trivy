using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

//TODO: change name
public class KubernetesNamespaceAddedHandler(
    IConcurrentCache<string, List<VulnerabilityReportCR>> cache,
    IVulnerabilityReportDomainService domainService)
    : IKubernetesNamespaceAddedHandler
{
    public async Task Handle(string k8sNamespace)
    {
        List<VulnerabilityReportCR> vulnerabilityReportCrList =
            await domainService.GetTrivyVulnerabilities(k8sNamespace);
        cache.TryAdd(k8sNamespace, vulnerabilityReportCrList);
    }
}
