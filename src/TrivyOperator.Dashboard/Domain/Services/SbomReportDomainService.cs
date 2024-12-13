using k8s;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Services;

public class SbomReportDomainService(IKubernetesClientFactory kubernetesClientFactory)
    : ISbomReportDomainService
{
    private readonly Kubernetes kubernetesClient = kubernetesClientFactory.GetClient();

    public async Task<IList<SbomReportCr>> GetSbomReportCrs()
    {
        SbomReportCrd myCrd = new();
        CustomResourceList<SbomReportCr> csr =
            await kubernetesClient.CustomObjects
                .ListNamespacedCustomObjectAsync<CustomResourceList<SbomReportCr>>(
                    myCrd.Group,
                    myCrd.Version,
                    "trivy",
                    myCrd.PluralName);

        return csr.Items ?? [];
    }
}
