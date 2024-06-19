using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public class KubernetesNamespaceDeletedHandler(IConcurrentCache<string, List<VulnerabilityReportCR>> cache)
    : IKubernetesNamespaceDeletedHandler
{
    public Task Handle(string k8sNamespace)
    {
        // TODO:
        return Task.CompletedTask;
    }
}
