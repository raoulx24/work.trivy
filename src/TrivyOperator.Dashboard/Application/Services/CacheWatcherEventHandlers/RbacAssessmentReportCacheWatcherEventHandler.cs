using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.RbacAssessmentReport;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;
public class RbacAssessmentReportCacheWatcherEventHandler(
    ICacheRefresh<RbacAssessmentReportCr, IBackgroundQueue<RbacAssessmentReportCr>> cacheRefresh,
    INamespacedWatcher<RbacAssessmentReportCr> kubernetesWatcher,
    ILogger<RbacAssessmentReportCacheWatcherEventHandler> logger)
    : NamespacedCacheWatcherEventHandler<IBackgroundQueue<RbacAssessmentReportCr>,
        ICacheRefresh<RbacAssessmentReportCr, IBackgroundQueue<RbacAssessmentReportCr>>,
        WatcherEvent<RbacAssessmentReportCr>, INamespacedWatcher<RbacAssessmentReportCr>, RbacAssessmentReportCr,
        CustomResourceList<RbacAssessmentReportCr>>(cacheRefresh, kubernetesWatcher, logger);
