using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;

public class ExposedSecretReportWatcherCacheSomething(
    ICacheRefresh<ExposedSecretReportCR, IBackgroundQueue<ExposedSecretReportCR>> cacheRefresh,
    INamespacedWatcher<ExposedSecretReportCR> kubernetesWatcher,
    ILogger<ExposedSecretReportWatcherCacheSomething> logger)
    : NamespacedWatcherCacheSomething<IBackgroundQueue<ExposedSecretReportCR>,
        ICacheRefresh<ExposedSecretReportCR, IBackgroundQueue<ExposedSecretReportCR>>,
        WatcherEvent<ExposedSecretReportCR>, INamespacedWatcher<ExposedSecretReportCR>, ExposedSecretReportCR,
        CustomResourceList<ExposedSecretReportCR>>(cacheRefresh, kubernetesWatcher, logger);
