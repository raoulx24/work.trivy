using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesNamespaceAddedHandler(IConcurrentCache<string, List<VulnerabilityReportCR>> cache)
    : IKubernetesNamespaceAddedHandler
{
    public Task Handle(string k8sNamespace)
    {
        // TODO:
        return Task.CompletedTask;
    }
}
