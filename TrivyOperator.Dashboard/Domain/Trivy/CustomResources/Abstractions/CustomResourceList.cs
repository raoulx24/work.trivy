namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions
{
    using k8s.Models;
    using k8s;

    public class CustomResourceList<T> : KubernetesObject
    where T : CustomResource
    {
        public V1ListMeta? Metadata { get; set; }
        public List<T>? Items { get; set; }
    }
}
