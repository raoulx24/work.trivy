namespace TrivyOperator.Dashboard.Infrastructure.Abstractions
{
    using k8s;

    public interface IK8sClientFactory
    {
        Kubernetes GetClient();
    }
}
