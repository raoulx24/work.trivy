using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;
public class ConfigAuditReportWatcherCacheSomething(
    ICacheRefresh<ConfigAuditReportCr, IBackgroundQueue<ConfigAuditReportCr>> cacheRefresh,
    INamespacedWatcher<ConfigAuditReportCr> kubernetesWatcher,
    ILogger<ConfigAuditReportWatcherCacheSomething> logger)
    : NamespacedWatcherCacheSomething<IBackgroundQueue<ConfigAuditReportCr>,
        ICacheRefresh<ConfigAuditReportCr, IBackgroundQueue<ConfigAuditReportCr>>,
        WatcherEvent<ConfigAuditReportCr>, INamespacedWatcher<ConfigAuditReportCr>, ConfigAuditReportCr,
        CustomResourceList<ConfigAuditReportCr>>(cacheRefresh, kubernetesWatcher, logger);
