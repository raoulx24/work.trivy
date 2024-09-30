using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class ClusterRbacAssessmentReportWatcher(
    IKubernetesClientFactory kubernetesClientFactory,
    IBackgroundQueue<ClusterRbacAssessmentReportCr> backgroundQueue,
    IWatcherState watcherState,
    ILogger<ClusterRbacAssessmentReportWatcher> logger)
    : ClusterScopedWatcher<CustomResourceList<ClusterRbacAssessmentReportCr>, ClusterRbacAssessmentReportCr,
        IBackgroundQueue<ClusterRbacAssessmentReportCr>, WatcherEvent<ClusterRbacAssessmentReportCr>>(
        kubernetesClientFactory,
        backgroundQueue,
        watcherState,
        logger)
{
    protected override async Task<HttpOperationResponse<CustomResourceList<ClusterRbacAssessmentReportCr>>>
        GetKubernetesObjectWatchList(
            IKubernetesObject<V1ObjectMeta>? sourceKubernetesObject,
            CancellationToken cancellationToken)
    {
        ClusterRbacAssessmentReportCrd myCrd = new();

        return await KubernetesClient.CustomObjects
            .ListClusterCustomObjectWithHttpMessagesAsync<CustomResourceList<ClusterRbacAssessmentReportCr>>(
                myCrd.Group,
                myCrd.Version,
                myCrd.PluralName,
                watch: true,
                timeoutSeconds: int.MaxValue,
                cancellationToken: cancellationToken);
    }
}
