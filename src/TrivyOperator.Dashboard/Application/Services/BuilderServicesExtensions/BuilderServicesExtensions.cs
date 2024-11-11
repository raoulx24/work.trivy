using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Alerts;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Application.Services.Watchers;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Domain.Services;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterComplianceReport;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Clients;
using TrivyOperator.Dashboard.Infrastructure.Services;

namespace TrivyOperator.Dashboard.Application.Services.BuilderServicesExtensions;

public static class BuilderServicesExtensions
{
    public static void AddV1NamespaceServices(this IServiceCollection services, IConfiguration kubernetesConfiguration)
    {
        services
            .AddSingleton<IConcurrentCache<string, IList<V1Namespace>>, ConcurrentCache<string, IList<V1Namespace>>>();
        services.AddSingleton<IBackgroundQueue<V1Namespace>, BackgroundQueue<V1Namespace>>();
        if (string.IsNullOrWhiteSpace(kubernetesConfiguration.GetValue<string>("NamespaceList")))
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

    public static void AddClusterRbacAssessmentReportServices(
        this IServiceCollection services,
        IConfiguration kubernetesConfiguration)
    {
        bool? useServices = kubernetesConfiguration.GetValue<bool?>("TrivyUseClusterRbacAssessmentReport");
        if (useServices == null || !(bool)useServices)
        {
            services.AddScoped<IClusterRbacAssessmentReportService, ClusterRbacAssessmentReportNullService>();
            return;
        }

        services.AddSingleton<
            IConcurrentCache<string, IList<ClusterRbacAssessmentReportCr>>,
            ConcurrentCache<string, IList<ClusterRbacAssessmentReportCr>>>();
        services
            .AddSingleton<IBackgroundQueue<ClusterRbacAssessmentReportCr>,
                BackgroundQueue<ClusterRbacAssessmentReportCr>>();
        services
            .AddSingleton<IClusterScopedWatcher<ClusterRbacAssessmentReportCr>, ClusterRbacAssessmentReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<ClusterRbacAssessmentReportCr, IBackgroundQueue<ClusterRbacAssessmentReportCr>>, CacheRefresh<
                ClusterRbacAssessmentReportCr, IBackgroundQueue<ClusterRbacAssessmentReportCr>>>();
        services
            .AddSingleton<IClusterScopedCacheWatcherEventHandler, ClusterRbacAssessmentReportWatcherEventHandler>();
        services.AddScoped<IClusterRbacAssessmentReportService, ClusterRbacAssessmentReportService>();
    }

    public static void AddConfigAuditReportServices(
        this IServiceCollection services,
        IConfiguration kubernetesConfiguration)
    {
        bool? useServices = kubernetesConfiguration.GetValue<bool?>("TrivyUseConfigAuditReport");
        if (useServices == null || !(bool)useServices)
        {
            services.AddScoped<IConfigAuditReportService, ConfigAuditReportNullService>();
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
        services.AddScoped<IConfigAuditReportService, ConfigAuditReportService>();
    }

    public static void AddExposedSecretReportServices(
        this IServiceCollection services,
        IConfiguration kubernetesConfiguration)
    {
        bool? useServices = kubernetesConfiguration.GetValue<bool?>("TrivyUseConfigAuditReport");
        if (useServices == null || !(bool)useServices)
        {
            services.AddScoped<IExposedSecretReportService, ExposedSecretReportNullService>();
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
        services.AddScoped<IExposedSecretReportService, ExposedSecretReportService>();
    }

    public static void AddVulnerabilityReportServices(
        this IServiceCollection services,
        IConfiguration kubernetesConfiguration)
    {
        bool? useServices = kubernetesConfiguration.GetValue<bool?>("TrivyUseVulnerabilityReport");
        if (useServices == null || !(bool)useServices)
        {
            services.AddScoped<IVulnerabilityReportService, VulnerabilityReportNullService>();
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

    public static void AddClusterComplianceReportServices(
        this IServiceCollection services,
        IConfiguration kubernetesConfiguration)
    {
        bool? useServices = kubernetesConfiguration.GetValue<bool?>("TrivyUseClusterComplianceReport");

        if (useServices == null || !(bool)useServices)
        {
//            services.AddScoped<IClusterComplianceReportService, ClusterComplianceReportNullService>();
            return;
        }

        services.AddSingleton<
            IConcurrentCache<string, IList<ClusterComplianceReportCr>>,
            ConcurrentCache<string, IList<ClusterComplianceReportCr>>>();
        services
            .AddSingleton<IBackgroundQueue<ClusterComplianceReportCr>,
                BackgroundQueue<ClusterComplianceReportCr>>();
        services
            .AddSingleton<IClusterScopedWatcher<ClusterComplianceReportCr>, ClusterComplianceReportWatcher>();
        services.AddSingleton<
            ICacheRefresh<ClusterComplianceReportCr, IBackgroundQueue<ClusterComplianceReportCr>>, CacheRefresh<
                ClusterComplianceReportCr, IBackgroundQueue<ClusterComplianceReportCr>>>();
        services
            .AddSingleton<IClusterScopedCacheWatcherEventHandler, ClusterComplianceReportWatcherEventHandler>();
        //services.AddScoped<IClusterComplianceReportService, ClusterComplianceReportService>();
    }

    public static void AddWatcherStateServices(this IServiceCollection services)
    {
        services.AddTransient<IWatcherState, WatcherState>();
        services.AddSingleton<IConcurrentCache<string, WatcherStateInfo>, ConcurrentCache<string, WatcherStateInfo>>();

        services.AddScoped<IWatcherStateInfoService, WatcherStateInfoService>();
    }

    public static void AddAlertsServices(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IConcurrentCache<string, IList<Alert>>, ConcurrentCache<string, IList<Alert>>>();
        services.AddTransient<IAlertsService, AlertsService>();
    }

    public static void AddCommons(
        this IServiceCollection services,
        IConfiguration queuesConfiguration,
        IConfiguration kubernetesConfiguration)
    {
        services.Configure<BackgroundQueueOptions>(queuesConfiguration);
        services.Configure<KubernetesOptions>(kubernetesConfiguration);

        services.AddHostedService<CacheWatcherEventHandlerHostedService>();

        services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();
    }

    public static void AddUiCommons(this IServiceCollection services) =>
        services.AddScoped<IBackendSettingsService, BackendSettingsService>();
}
