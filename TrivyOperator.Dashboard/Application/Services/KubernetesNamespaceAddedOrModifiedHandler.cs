﻿using TrivyOperator.Dashboard.Application.Services.Abstractions;
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
        List<VulnerabilityReportCR> vulnerabilityReportCrList =
            await domainService.GetTrivyVulnerabilities(k8sNamespace);
        cache[k8sNamespace] = vulnerabilityReportCrList;
        logger.LogInformation(
            "VulnerabilityReport Cache item added/updated for namespace {k8sNamespace}",
            k8sNamespace);
    }
}