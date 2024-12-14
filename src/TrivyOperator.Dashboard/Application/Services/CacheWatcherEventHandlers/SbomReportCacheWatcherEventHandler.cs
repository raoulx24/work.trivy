using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

namespace TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;

public class SbomReportCacheWatcherEventHandler(
    ICacheRefresh<SbomReportCr, IBackgroundQueue<SbomReportCr>> cacheRefresh,
    INamespacedWatcher<SbomReportCr> kubernetesWatcher,
    ILogger<SbomReportCacheWatcherEventHandler> logger)
    : NamespacedCacheWatcherEventHandler<IBackgroundQueue<SbomReportCr>,
        ICacheRefresh<SbomReportCr, IBackgroundQueue<SbomReportCr>>,
        WatcherEvent<SbomReportCr>, INamespacedWatcher<SbomReportCr>, SbomReportCr,
        CustomResourceList<SbomReportCr>>(cacheRefresh, kubernetesWatcher, logger);
