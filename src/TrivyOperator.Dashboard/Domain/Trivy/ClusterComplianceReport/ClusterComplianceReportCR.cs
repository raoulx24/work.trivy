using k8s;
using k8s.Models;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterComplianceReport;

public class ClusterComplianceReportCr : CustomResource, IKubernetesObject<V1ObjectMeta>
{
    [JsonPropertyName("spec")]
    public Spec Spec { get; init; } = new();

    [JsonPropertyName("status")]
    public Status Status { get; init; } = new();
}
