namespace TrivyOperator.Dashboard.Infrastructure.Clients
{
    using k8s;
    using TrivyOperator.Dashboard.Infrastructure.Abstractions;

    public class K8sClientFactory : IK8sClientFactory
    {
        private Kubernetes k8sClient { get; set; }

        public K8sClientFactory()
        {
            KubernetesClientConfiguration config = KubernetesClientConfiguration.IsInCluster() ?
                KubernetesClientConfiguration.InClusterConfig() :
                KubernetesClientConfiguration.BuildConfigFromConfigFile();
            k8sClient = new Kubernetes(config);
        }

        public Kubernetes GetClient()
        {
            return k8sClient;
        }

    }
}
