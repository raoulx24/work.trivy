using System.Text.Json.Serialization;
using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

public abstract class CustomResource : KubernetesObject, IMetadata<V1ObjectMeta>
{
    [JsonPropertyName("metadata")]
    public V1ObjectMeta Metadata { get; set; }
}
