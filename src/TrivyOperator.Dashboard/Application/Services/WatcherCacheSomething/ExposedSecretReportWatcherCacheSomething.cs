using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

namespace TrivyOperator.Dashboard.Application.Services.WatcherCacheSomething;

public class ExposedSecretReportWatcherCacheSomething(
    ICacheRefresh<ExposedSecretReportCr, IBackgroundQueue<ExposedSecretReportCr>> cacheRefresh,
    INamespacedWatcher<ExposedSecretReportCr> kubernetesWatcher,
    ILogger<ExposedSecretReportWatcherCacheSomething> logger)
    : NamespacedWatcherCacheSomething<IBackgroundQueue<ExposedSecretReportCr>,
        ICacheRefresh<ExposedSecretReportCr, IBackgroundQueue<ExposedSecretReportCr>>,
        WatcherEvent<ExposedSecretReportCr>, INamespacedWatcher<ExposedSecretReportCr>, ExposedSecretReportCr,
        CustomResourceList<ExposedSecretReportCr>>(cacheRefresh, kubernetesWatcher, logger);
