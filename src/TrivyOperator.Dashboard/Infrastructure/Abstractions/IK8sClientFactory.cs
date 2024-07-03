using k8s;

namespace TrivyOperator.Dashboard.Infrastructure.Abstractions;

public interface IKubernetesClientFactory
{
    Kubernetes GetClient();
}
