using k8s;
using k8s.Models;

namespace TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

public class CustomResourceList<T> : KubernetesObject, IItems<T> where T : CustomResource
{
    public V1ListMeta? Metadata { get; set; }
    public IList<T>? Items { get; set; }
}
