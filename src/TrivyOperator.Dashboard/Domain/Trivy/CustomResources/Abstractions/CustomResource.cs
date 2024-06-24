using k8s;
using k8s.Models;
using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

public abstract class CustomResource : KubernetesObject, IMetadata<V1ObjectMeta>
{
    [JsonPropertyName("metadata")]
    public V1ObjectMeta Metadata { get; set; }
}
