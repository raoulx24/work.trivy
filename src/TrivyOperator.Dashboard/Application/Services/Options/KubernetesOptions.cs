namespace TrivyOperator.Dashboard.Application.Services.Options;

public record KubernetesOptions
{
    public string? KubeConfigFileName { get; init; }
    public string? NamespaceList { get; init; }
    public bool? TrivyUseClusterRbacAssessmentReport { get; init; }
    public bool? TrivyUseConfigAuditReport { get; init; }
    public bool? TrivyUseExposedSecretReport { get; init; }
    public bool? TrivyUseVulnerabilityReport { get; init; }
    public bool? TrivyUseClusterVulnerabilityReport { get; init; }
    public bool? TrivyUseRbacAssessmentReport { get; init; }
}
