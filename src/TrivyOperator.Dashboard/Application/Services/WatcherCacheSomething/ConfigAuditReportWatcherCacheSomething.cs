using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;
public class ConfigAuditReportWatcherCacheSomething(
    ICacheRefresh<ConfigAuditReportCR, IBackgroundQueue<ConfigAuditReportCR>> cacheRefresh,
    INamespacedWatcher<ConfigAuditReportCR> kubernetesWatcher,
    ILogger<ConfigAuditReportWatcherCacheSomething> logger)
    : NamespacedWatcherCacheSomething<IBackgroundQueue<ConfigAuditReportCR>,
        ICacheRefresh<ConfigAuditReportCR, IBackgroundQueue<ConfigAuditReportCR>>,
        WatcherEvent<ConfigAuditReportCR>, INamespacedWatcher<ConfigAuditReportCR>, ConfigAuditReportCR,
        CustomResourceList<ConfigAuditReportCR>>(cacheRefresh, kubernetesWatcher, logger);
