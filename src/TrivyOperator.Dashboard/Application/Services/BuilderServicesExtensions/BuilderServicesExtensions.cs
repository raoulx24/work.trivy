using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Services;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Alerts;

namespace TrivyOperator.Dashboard.Application.Services.BuilderServicesExtensions;

public static class BuilderServicesExtensions
{
    public static void AddV1NamespaceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConcurrentCache<string, IList<V1Namespace>>, ConcurrentCache<string, IList<V1Namespace>>>();
        services.AddSingleton<IBackgroundQueue<V1Namespace>, BackgroundQueue<V1Namespace>>();
        if (string.IsNullOrWhiteSpace(configuration.GetSection("Kubernetes").GetValue<String>("NamespaceList")))
        {
            services.AddSingleton<IClusterScopedWatcher<V1Namespace>, NamespaceWatcher>();
        }
        else
        {
            services.AddSingleton<IKubernetesNamespaceDomainService, StaticKubernetesNamespaceDomainService>();
            services.AddSingleton<IClusterScopedWatcher<V1Namespace>, StaticNamespaceWatcher>();
        }
        services.AddSingleton<ICacheRefresh<V1Namespace, IBackgroundQueue<V1Namespace>>, NamespaceCacheRefresh>();
        services.AddSingleton<IClusterScopedCacheWatcherEventHandler, NamespaceCacheWatcherEventHandler>();

        services.AddScoped<INamespaceService, NamespaceService>();
    }

    public static void AddClusterRbacAssessmentReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        bool? useServices = configuration.GetValue<bool?>("TrivyUseClusterRbacAssessmentReport");
        if (useServices == null || !(bool)useServices)
        {
            return;
        }
        services.AddSingleton<
            IConcurrentCache<string, IList<ClusterRbacAssessmentReportCr>>,
            ConcurrentCache<string, IList<ClusterRbacAssessmentReportCr>>>();
        services.AddSingleton<IBackgroundQueue<ClusterRbacAssessmentReportCr>, BackgroundQueue<ClusterRbacAssessmentReportCr>>();
        services.AddSingleton<IClusterScopedWatcher<ClusterRbacAssessmentReportCr>, ClusterRbacAssessmentReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<ClusterRbacAssessmentReportCr, IBackgroundQueue<ClusterRbacAssessmentReportCr>>,
            CacheRefresh<ClusterRbacAssessmentReportCr, IBackgroundQueue<ClusterRbacAssessmentReportCr>>>();
        services.AddSingleton<IClusterScopedCacheWatcherEventHandler, ClusterRbacAssessmentReportWatcherCacheSomething>();
    }

    public static void AddConfigAuditReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        bool? useServices = configuration.GetValue<bool?>("TrivyUseConfigAuditReport");
        if (useServices == null || !(bool)useServices)
        {
            return;
        }
        services.AddSingleton<
            IConcurrentCache<string, IList<ConfigAuditReportCr>>,
            ConcurrentCache<string, IList<ConfigAuditReportCr>>>();
        services.AddSingleton<IBackgroundQueue<ConfigAuditReportCr>, BackgroundQueue<ConfigAuditReportCr>>();
        services.AddSingleton<INamespacedWatcher<ConfigAuditReportCr>, ConfigAuditReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<ConfigAuditReportCr, IBackgroundQueue<ConfigAuditReportCr>>,
            CacheRefresh<ConfigAuditReportCr, IBackgroundQueue<ConfigAuditReportCr>>>();
        services.AddSingleton<INamespacedCacheWatcherEventHandler, ConfigAuditReportCacheWatcherEventHandler>();
    }

    public static void AddExposedSecretReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        bool? useServices = configuration.GetValue<bool?>("TrivyUseConfigAuditReport");
        if (useServices == null || !(bool)useServices)
        {
            return;
        }
        services.AddSingleton<
            IConcurrentCache<string, IList<ExposedSecretReportCr>>,
            ConcurrentCache<string, IList<ExposedSecretReportCr>>>();
        services.AddSingleton<IBackgroundQueue<ExposedSecretReportCr>, BackgroundQueue<ExposedSecretReportCr>>();
        services.AddSingleton<INamespacedWatcher<ExposedSecretReportCr>, ExposedSecretReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<ExposedSecretReportCr, IBackgroundQueue<ExposedSecretReportCr>>,
            CacheRefresh<ExposedSecretReportCr, IBackgroundQueue<ExposedSecretReportCr>>>();
        services.AddSingleton<INamespacedCacheWatcherEventHandler, ExposedSecretReportCacheWatcherEventHandler>();
    }

    public static void AddVulnerabilityReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        bool? useServices = configuration.GetValue<bool?>("TrivyUseVulnerabilityReport");
        if (useServices == null || !(bool)useServices)
        {
            return;
        }
        services.AddSingleton<
            IConcurrentCache<string, IList<VulnerabilityReportCr>>,
            ConcurrentCache<string, IList<VulnerabilityReportCr>>>();
        services.AddSingleton<IBackgroundQueue<VulnerabilityReportCr>, BackgroundQueue<VulnerabilityReportCr>>();
        services.AddSingleton<INamespacedWatcher<VulnerabilityReportCr>, VulnerabilityReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<VulnerabilityReportCr, IBackgroundQueue<VulnerabilityReportCr>>,
            CacheRefresh<VulnerabilityReportCr, IBackgroundQueue<VulnerabilityReportCr>>>();
        services.AddSingleton<INamespacedCacheWatcherEventHandler, VulnerabilityReportCacheWatcherEventHandler>();

        services.AddScoped<IVulnerabilityReportService, VulnerabilityReportService>();
    }

    public static void AddWatcherStateServices(this IServiceCollection services)
    {
        services.AddTransient<IWatcherState, WatcherState>();
        services.AddSingleton<IConcurrentCache<string, WatcherStateInfo>, ConcurrentCache<string, WatcherStateInfo>>();

        services.AddScoped<IWatcherStateInfoService, WatcherStateInfoService>();
    }

    public static void AddAlertsServices(this IServiceCollection services)
    {
        services.AddSingleton<IConcurrentCache<string, IList<Alert>>, ConcurrentCache<string, IList<Alert>>>();
        services.AddTransient<IAlertsService, AlertsService>();
    }
}
