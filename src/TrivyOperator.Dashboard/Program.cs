using k8s;
using k8s.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacherRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers;
using TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherEvents.Abstractions;
using TrivyOperator.Dashboard.Domain.Services;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Clients;
using TrivyOperator.Dashboard.Infrastructure.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

const string applicationName = "TrivyOperator.Dashboard";
WebApplicationBuilder builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        ApplicationName = applicationName,
        ContentRootPath = Directory.GetCurrentDirectory(),
        WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
    });

IConfiguration configuration = CreateConfiguration();
builder.Configuration.Sources.Clear();
builder.Configuration.AddConfiguration(configuration);

LoggerConfiguration loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(configuration);
loggerConfiguration.Enrich.FromLogContext();
loggerConfiguration.Enrich.WithMachineName();
loggerConfiguration.Enrich.WithThreadId();
loggerConfiguration.Enrich.WithProperty("Application", applicationName);
Log.Logger = loggerConfiguration.CreateLogger();
SerilogLoggerFactory serilogLoggerFactory = new(Log.Logger);
Logger = serilogLoggerFactory.CreateLogger<Program>();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;

builder.Host.UseSerilog(Log.Logger);

builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(10));
builder.WebHost.ConfigureKestrel(options => { options.AddServerHeader = false; });

builder.Services.AddControllersWithViews()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
builder.Services
    .AddHttpClient(); // see: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
builder.Services.Configure<ForwardedHeadersOptions>(
    options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto |
                                   ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(
    options => options.AddDefaultPolicy(
        configurePolicy => configurePolicy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

# region New Services

//builder.Services
//    .AddSingleton<IConcurrentCache<string, IList<V1Namespace>>, ConcurrentCache<string, IList<V1Namespace>>>();
//builder.Services.AddSingleton<IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>>(
//    x => new BackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>(100));
//builder.Services
//    .AddSingleton<
//        IKubernetesWatcher<V1NamespaceList, V1Namespace, IKubernetesObject<V1ObjectMeta>,
//            IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>, KubernetesWatcherEvent<V1Namespace>>,
//        NamespaceWatcher>();
//builder.Services
//    .AddSingleton<
//        ICacheRefresh<V1Namespace, KubernetesWatcherEvent<V1Namespace>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>>,
//        CacheRefresh<V1Namespace, KubernetesWatcherEvent<V1Namespace>, IBackgroundQueue<KubernetesWatcherEvent<V1Namespace>, V1Namespace>>>();
//builder.Services.AddHostedService<CacheRefresh<VulnerabilityReportCR, KubernetesWatcherEvent<VulnerabilityReportCR>, BackgroundQueue<KubernetesWatcherEvent<VulnerabilityReportCR>, VulnerabilityReportCR>>>();

#endregion


#region Old Services

builder.Services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();

builder.Services
    .AddSingleton<IConcurrentCache<string, IList<VulnerabilityReportCR>>,
        ConcurrentCache<string, IList<VulnerabilityReportCR>>>();
builder.Services.AddSingleton<IConcurrentCache<string, DateTime>, ConcurrentCache<string, DateTime>>();

builder.Services.AddScoped<IVulnerabilityReportService, VulnerabilityReportService>();
builder.Services.AddScoped<IVulnerabilityReportDomainService, VulnerabilityReportDomainService>();

builder.Services.AddScoped<IKubernetesNamespaceService, KubernetesNamespaceService>();
if (string.IsNullOrWhiteSpace(configuration.GetSection("Kubernetes").GetValue<string>("NamespaceList")))
{
    builder.Services.AddScoped<IKubernetesNamespaceDomainService, KubernetesNamespaceDomainService>();
}
else
{
    builder.Services.AddScoped<IKubernetesNamespaceDomainService, StaticKubernetesNamespaceDomainService>();
}

builder.Services.AddScoped<IKubernetesNamespaceAddedOrModifiedHandler, KubernetesNamespaceAddedOrModifiedHandler>();
builder.Services.AddScoped<IKubernetesNamespaceDeletedHandler, KubernetesNamespaceDeletedHandler>();
builder.Services
    .AddScoped<IKubernetesVulnerabilityReportCrWatchEventHandler, KubernetesVulnerabilityReportCrWatchEventHandler>();

builder.Services.AddHostedService<KubernetesHostedService>();
builder.Services.AddHostedService<CacheRefreshHostedService>();

# endregion

WebApplication app = builder.Build();

IHostApplicationLifetime appLifetime = app.Lifetime;
appLifetime.ApplicationStarted.Register(OnStarted);
appLifetime.ApplicationStopping.Register(OnStopping);
appLifetime.ApplicationStopped.Register(OnStopped);

// Configure the HTTP request pipeline. Middleware order: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
app.UseForwardedHeaders();
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseSerilogRequestLogging();
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapFallbackToFile("index.html");

await app.RunAsync().ConfigureAwait(false);
return 0;

static IConfiguration CreateConfiguration()
{
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
        .AddJsonFile("serilog.config.json", true)
        .AddEnvironmentVariables();
    return configurationBuilder.Build();
}

static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    if (e.ExceptionObject is Exception ex)
    {
        Logger?.LogError(ex, "UnhandledException");
    }
    else
    {
        string? msg = e.ExceptionObject.ToString();
        int exCode = Marshal.GetLastWin32Error();
        if (exCode != 0)
        {
            msg += " ErrorCode: " + exCode.ToString("X16");
        }

        Logger?.LogError($"Unhandled External Exception: {msg}");
    }
}

static void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
{
    Logger?.LogError(e.Exception, "ERROR: UNOBSERVED TASK EXCEPTION");
    e.SetObserved();
}

static void OnStarted() => Logger?.LogInformation("OnStarted has been called.");

static void OnStopping() => Logger?.LogInformation("OnStopping has been called.");

static void OnStopped()
{
    Logger?.LogInformation("OnStopped has been called.");
    Log.CloseAndFlush();
}

internal partial class Program
{
    private static ILogger? Logger { get; set; }
}
