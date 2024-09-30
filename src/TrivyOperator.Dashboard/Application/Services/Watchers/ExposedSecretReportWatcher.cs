using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherState;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class ExposedSecretReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<ExposedSecretReportCr> backgroundQueue,
    IWatcherState watcherState,
    ILogger<ExposedSecretReportWatcher> logger)
    : NamespacedWatcher<CustomResourceList<ExposedSecretReportCr>, ExposedSecretReportCr,
        IBackgroundQueue<ExposedSecretReportCr>, WatcherEvent<ExposedSecretReportCr>>(
        kubernetesClientFactory,
        backgroundQueue,
        watcherState,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<ExposedSecretReportCr>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        ExposedSecretReportCrd myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<ExposedSecretReportCr>>(
                myCrd.Group,
                myCrd.Version,
                GetNamespaceFromSourceEvent(sourceKubernetesObject),
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }
}
