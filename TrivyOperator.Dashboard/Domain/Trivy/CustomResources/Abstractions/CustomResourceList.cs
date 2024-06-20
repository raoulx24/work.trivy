using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

public class CustomResourceList<T> : KubernetesObject where T : CustomResource
{
    public V1ListMeta? Metadata { get; set; }
    public List<T>? Items { get; set; }
}
