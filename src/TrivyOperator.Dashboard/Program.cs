using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Application.Hubs;
using TrivyOperator.Dashboard.Application.Services.BuilderServicesExtensions;
using TrivyOperator.Dashboard.Utils;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

const string applicationName = "TrivyOperator.Dashboard";
const string queuesConfigurationSectionKey = "Queues";
const string kubernetesConfigurationSectionKey = "Kubernetes";
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
builder.Services.Configure<ForwardedHeadersOptions>(
    options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto |
                                   ForwardedHeaders.XForwardedHost;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

builder.Services.AddControllersWithViews(ConfigureMvcOptions)
    .AddJsonOptions(options => ConfigureJsonSerializerOptions(options.JsonSerializerOptions));
builder.Services.AddHttpClient();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SignalR: CORS with credentials must be allowed in order for cookie-based sticky sessions to work correctly. They must be enabled even when authentication isn't used.
builder.Services.AddCors(
    options => options.AddDefaultPolicy(
        configurePolicy => configurePolicy.SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

builder.Services.AddCommons(
    configuration.GetSection(queuesConfigurationSectionKey),
    configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddAlertsServices();
builder.Services.AddWatcherStateServices();
builder.Services.AddV1NamespaceServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddClusterRbacAssessmentReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddConfigAuditReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddExposedSecretReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddVulnerabilityReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddClusterComplianceReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddClusterVulnerabilityReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddRbacAssessmentReportServices(configuration.GetSection(kubernetesConfigurationSectionKey));
builder.Services.AddUiCommons();

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
app.MapHub<AlertsHub>("/alerts-hub");
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
