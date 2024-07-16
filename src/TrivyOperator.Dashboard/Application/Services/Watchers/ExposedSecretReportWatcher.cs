using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class ExposedSecretReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<ExposedSecretReportCR> backgroundQueue,
    ILogger<ExposedSecretReportWatcher> logger)
    : NamespacedWatcher<CustomResourceList<ExposedSecretReportCR>, ExposedSecretReportCR,
        IBackgroundQueue<ExposedSecretReportCR>, WatcherEvent<ExposedSecretReportCR>>(
        kubernetesClientFactory,
        backgroundQueue,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<ExposedSecretReportCR>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        ExposedSecretReportCRD myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<ExposedSecretReportCR>>(
                myCrd.Group,
                myCrd.Version,
                GetNamespaceFromSourceEvent(sourceKubernetesObject),
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }
}
