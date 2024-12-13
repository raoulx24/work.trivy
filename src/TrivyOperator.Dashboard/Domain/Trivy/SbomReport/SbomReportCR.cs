using k8s;
using k8s.Models;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

public class SbomReportCr : CustomResource, IKubernetesObject<V1ObjectMeta>
{
    [JsonPropertyName("report")]
    public Report? Report { get; init; }
}
