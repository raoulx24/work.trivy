using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;

public class ConfigAuditReportCrd : CustomResourceDefinition
{
    public override string Version { get; } = "v1alpha1";
    public override string Group { get; } = "aquasecurity.github.io";
    public override string PluralName { get; } = "configauditreports";
    public override string Kind { get; } = "CResource";
    public override string? Namespace { get; init; } = "default";
}
