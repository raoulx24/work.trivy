using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterComplianceReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;

public class ClusterComplianceReportWatcherEventHandler(
    ICacheRefresh<ClusterComplianceReportCr, IBackgroundQueue<ClusterComplianceReportCr>> cacheRefresh,
    IClusterScopedWatcher<ClusterComplianceReportCr> kubernetesWatcher,
    ILogger<ClusterComplianceReportWatcherEventHandler> logger)
    : ClusterScopedCacheWatcherEventHandler<IBackgroundQueue<ClusterComplianceReportCr>,
        ICacheRefresh<ClusterComplianceReportCr, IBackgroundQueue<ClusterComplianceReportCr>>,
        WatcherEvent<ClusterComplianceReportCr>, IClusterScopedWatcher<ClusterComplianceReportCr>,
        ClusterComplianceReportCr, CustomResourceList<ClusterComplianceReportCr>>(
        cacheRefresh,
        kubernetesWatcher,
        logger);
