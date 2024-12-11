using k8s;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class ClusterSbomReportDomainService(IKubernetesClientFactory kubernetesClientFactory)
    : IClusterSbomReportDomainService
{
    private readonly Kubernetes kubernetesClient = kubernetesClientFactory.GetClient();

    public async Task<IList<ClusterSbomReportCr>> GetClusterSbomReportCrs()
    {
        ClusterSbomReportCrd myCrd = new();
        CustomResourceList<ClusterSbomReportCr> csr =
            await kubernetesClient.CustomObjects
                .ListNamespacedCustomObjectAsync<CustomResourceList<ClusterSbomReportCr>>(
                    myCrd.Group,
                    myCrd.Version,
                    "trivy",
                    myCrd.PluralName);

        return csr.Items ?? [];
    }
}
