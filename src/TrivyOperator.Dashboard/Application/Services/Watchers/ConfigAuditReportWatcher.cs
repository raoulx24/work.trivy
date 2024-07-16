using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class ConfigAuditReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<ConfigAuditReportCR> backgroundQueue,
    ILogger<ConfigAuditReportWatcher> logger)
    : NamespacedWatcher<CustomResourceList<ConfigAuditReportCR>, ConfigAuditReportCR,
        IBackgroundQueue<ConfigAuditReportCR>, WatcherEvent<ConfigAuditReportCR>>(
        kubernetesClientFactory,
        backgroundQueue,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<ConfigAuditReportCR>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        ConfigAuditReportCRD myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<ConfigAuditReportCR>>(
                myCrd.Group,
                myCrd.Version,
                GetNamespaceFromSourceEvent(sourceKubernetesObject),
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }
}
