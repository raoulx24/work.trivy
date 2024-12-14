using k8s.Autorest;
using k8s.Models;
using k8s;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class SbomReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<SbomReportCr> backgroundQueue,
    IServiceProvider serviceProvider,
    ILogger<SbomReportWatcher> logger)
    : NamespacedWatcher<CustomResourceList<SbomReportCr>, SbomReportCr,
        IBackgroundQueue<SbomReportCr>, WatcherEvent<SbomReportCr>>(
        kubernetesClientFactory,
        backgroundQueue,
        serviceProvider,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<SbomReportCr>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        SbomReportCrd myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<SbomReportCr>>(
                myCrd.Group,
                myCrd.Version,
                GetNamespaceFromSourceEvent(sourceKubernetesObject),
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }

    protected override void ProcessReceivedKubernetesObject(SbomReportCr kubernetesObject)
    {
        if (kubernetesObject.Report != null)
        {
            kubernetesObject.Report.Components.ComponentsComponents = [];
            kubernetesObject.Report.Components.Dependencies = [];
        }
        base.ProcessReceivedKubernetesObject(kubernetesObject);
    }
}
