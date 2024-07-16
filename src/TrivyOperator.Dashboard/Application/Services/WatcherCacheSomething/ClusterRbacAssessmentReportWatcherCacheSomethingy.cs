using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;
public class ClusterRbacAssessmentReportWatcherCacheSomething(
    ICacheRefresh<ClusterRbacAssessmentReportCR, IBackgroundQueue<ClusterRbacAssessmentReportCR>> cacheRefresh,
    IClusterScopedWatcher<ClusterRbacAssessmentReportCR> kubernetesWatcher,
    ILogger<ClusterRbacAssessmentReportWatcherCacheSomething> logger)
    : ClusterScopedWatcherCacheSomething<IBackgroundQueue<ClusterRbacAssessmentReportCR>,
        ICacheRefresh<ClusterRbacAssessmentReportCR, IBackgroundQueue<ClusterRbacAssessmentReportCR>>, WatcherEvent<ClusterRbacAssessmentReportCR>,
        IClusterScopedWatcher<ClusterRbacAssessmentReportCR>, ClusterRbacAssessmentReportCR, CustomResourceList<ClusterRbacAssessmentReportCR>>(cacheRefresh, kubernetesWatcher, logger);
