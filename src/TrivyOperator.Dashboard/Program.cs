using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Application.Hubs;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.BuilderServicesExtensions;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Clients;
using TrivyOperator.Dashboard.Utils;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

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
builder.Services.AddControllersWithViews(ConfigureMvcOptions)
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

builder.Services.AddSignalR();

builder.Services.AddAlertsServices();

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
builder.Services.AddScoped<IBackendSettingsService, BackendSettingsService>();

WebApplication app = builder.Build();

IHostApplicationLifetime appLifetime = app.Lifetime;
appLifetime.ApplicationStarted.Register(OnStarted);
appLifetime.ApplicationStopping.Register(OnStopping);
appLifetime.ApplicationStopped.Register(OnStopped);

app.MapHub<AlertsHub>("/alerts-hub");

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

static void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
{
    options.Converters.Add(new JsonStringEnumConverter());
    options.Converters.Add(new DateTimeJsonConverter());
    options.Converters.Add(new DateTimeNullableJsonConverter());
}

static void ConfigureMvcOptions(MvcOptions options) => options.Filters.Add(new ProducesAttribute("application/json"));

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

static void OnStarted()
{
    Logger?.LogInformation("OnStarted has been called.");
}

static void OnStopping()
{
    Logger?.LogInformation("OnStopping has been called.");
}

static void OnStopped()
{
    Logger?.LogInformation("OnStopped has been called.");
    Log.CloseAndFlush();
}

internal partial class Program
{
    private static ILogger? Logger { get; set; }
}
