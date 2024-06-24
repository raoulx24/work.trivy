using k8s;

namespace TrivyOperator.Dashboard.Infrastructure.Abstractions;

public interface IK8sClientFactory
{
    Kubernetes GetClient();
}
