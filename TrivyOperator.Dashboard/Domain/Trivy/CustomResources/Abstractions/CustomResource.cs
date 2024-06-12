namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions
{
    using k8s.Models;
    using k8s;
    using System.Text.Json.Serialization;

    public abstract class CustomResource : KubernetesObject, IMetadata<V1ObjectMeta>
    {
        [JsonPropertyName("metadata")]
        public V1ObjectMeta Metadata { get; set; }
    }
}
