using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class ConfigAuditReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<ConfigAuditReportCr> backgroundQueue,
    IServiceProvider serviceProvider,
    ILogger<ConfigAuditReportWatcher> logger)
    : NamespacedWatcher<CustomResourceList<ConfigAuditReportCr>, ConfigAuditReportCr,
        IBackgroundQueue<ConfigAuditReportCr>, WatcherEvent<ConfigAuditReportCr>>(
        kubernetesClientFactory,
        backgroundQueue,
        serviceProvider,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<ConfigAuditReportCr>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        ConfigAuditReportCrd myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<ConfigAuditReportCr>>(
                myCrd.Group,
                myCrd.Version,
                GetNamespaceFromSourceEvent(sourceKubernetesObject),
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }
}
