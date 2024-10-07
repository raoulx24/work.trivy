using k8s.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues;
using TrivyOperator.Dashboard.Application.Services.BackgroundQueues.Abstractions;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh;
using TrivyOperator.Dashboard.Application.Services.CacheRefresh.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers;
using TrivyOperator.Dashboard.Application.Services.CacheWatcherEventHandlers.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Watchers;
using TrivyOperator.Dashboard.Application.Services.Watchers.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;
using TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;
using TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Clients;
using TrivyOperator.Dashboard.Infrastructure.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using TrivyOperator.Dashboard.Utils;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services;
using TrivyOperator.Dashboard.Application.Services.BuilderServicesExtensions;

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

builder.Services.Configure<JsonOptions>(options => ConfigureJsonSerializerOptions(options.SerializerOptions));
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => ConfigureJsonSerializerOptions(options.JsonSerializerOptions));
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

#region Kubernetes Related Services

builder.Services.Configure<BackgroundQueueOptions>(configuration.GetSection("Queues"));
builder.Services.Configure<KubernetesOptions>(configuration.GetSection("Kubernetes"));

builder.Services.AddHostedService<CacheWatcherEventHandlerHostedService>();

builder.Services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();

builder.Services.AddWatcherStateServices();
builder.Services.AddV1NamespaceServices(configuration.GetSection("Kubernetes"));
builder.Services.AddClusterRbacAssessmentReportServices(configuration.GetSection("Kubernetes"));
builder.Services.AddConfigAuditReportServices(configuration.GetSection("Kubernetes"));
builder.Services.AddExposedSecretReportServices(configuration.GetSection("Kubernetes"));
builder.Services.AddVulnerabilityReportServices(configuration.GetSection("Kubernetes"));

#endregion

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

static void ConfigureJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
{
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    jsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    jsonSerializerOptions.Converters.Add(new DateTimeNullableJsonConverter());
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
