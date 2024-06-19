using k8s;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Infrastructure.Clients;

public class K8sClientFactory : IK8sClientFactory
{
    private readonly Kubernetes k8sClient;

    public K8sClientFactory()
    {
        KubernetesClientConfiguration config = KubernetesClientConfiguration.IsInCluster()
            ? KubernetesClientConfiguration.InClusterConfig()
            : KubernetesClientConfiguration.BuildConfigFromConfigFile();
        k8sClient = new Kubernetes(config);
    }

    public Kubernetes GetClient()
    {
        return k8sClient;
    }
}
