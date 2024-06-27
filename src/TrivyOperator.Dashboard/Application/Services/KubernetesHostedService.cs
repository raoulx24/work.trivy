using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Collections.Concurrent;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public readonly struct TaskWithCts
{
    public Task Task { get; init; }
    public CancellationTokenSource Cts { get; init; }
}

public class KubernetesHostedService(
    IServiceProvider services,
    IK8sClientFactory k8sClientFactory,
    ILogger<KubernetesHostedService> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<string, TaskWithCts> watchVulnerabilityReportCrsTaskDict = new();
    private Kubernetes k8sClient = k8sClientFactory.GetClient();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Hosted Service running.");

        using IServiceScope scope = services.CreateScope();
        IKubernetesNamespaceDomainService kubernetesNamespaceDomainService = scope.ServiceProvider.GetServices<IKubernetesNamespaceDomainService>().First();
        if (kubernetesNamespaceDomainService.IsStaticList)
        {
            List<string> k8sNamespaces = await kubernetesNamespaceDomainService.GetKubernetesNamespaces();
            foreach (string k8sNamespace in k8sNamespaces)
            {
                foreach (IKubernetesNamespaceAddedOrModifiedHandler handler in scope.ServiceProvider
                             .GetServices<IKubernetesNamespaceAddedOrModifiedHandler>())
                {
                    await handler.Handle(k8sNamespace);
                }

                CreateWatchVulnerabilityReportCrsTask(k8sNamespace, stoppingToken);
            }
        }
        else
        {
            await WatchNamespaces(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }

    private async Task WatchVulnerabilityReportCrs(
        Kubernetes k8sClient,
        string k8sNamespace,
        CancellationToken cancellationToken)
    {
        VulnerabilityReportCRD myCrd = new();
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Task<HttpOperationResponse<CustomResourceList<VulnerabilityReportCR>>> listNamespacedCustomObjectResp =
                    k8sClient.CustomObjects
                        .ListNamespacedCustomObjectWithHttpMessagesAsync<CustomResourceList<VulnerabilityReportCR>>(
                            myCrd.Group,
                            myCrd.Version,
                            k8sNamespace,
                            myCrd.PluralName,
                            watch: true,
                            timeoutSeconds: int.MaxValue,
                            cancellationToken: cancellationToken);
                await foreach ((WatchEventType type, VulnerabilityReportCR item) in listNamespacedCustomObjectResp
                                   .WatchAsync<VulnerabilityReportCR, CustomResourceList<VulnerabilityReportCR>>(
                                       ex => logger.LogError(
                                           $"{nameof(WatchVulnerabilityReportCrs)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    using IServiceScope scope = services.CreateScope();
                    foreach (IKubernetesVulnerabilityReportCrWatchEventHandler handler in scope.ServiceProvider
                                 .GetServices<IKubernetesVulnerabilityReportCrWatchEventHandler>())
                    {
                        await handler.Handle(type, item);
                    }
                }
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(WatchVulnerabilityReportCrs)} - {ex.Message}", ex);
            }
        }
    }

    private void CreateWatchVulnerabilityReportCrsTask(
        string k8sNamespace,
        CancellationToken stoppingToken)
    {
        CancellationTokenSource cts = new();
        TaskWithCts taskWithCts = new()
        {
            Task = WatchVulnerabilityReportCrs(
                k8sClient,
                k8sNamespace,
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token).Token),
            Cts = cts,
        };
        watchVulnerabilityReportCrsTaskDict[k8sNamespace] = taskWithCts;
    }

    private async Task WatchNamespaces(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Task<HttpOperationResponse<V1NamespaceList>> listNamespaceResp =
                    k8sClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
                        watch: true,
                        timeoutSeconds: int.MaxValue,
                        cancellationToken: stoppingToken);
                await foreach ((WatchEventType type, V1Namespace item) in listNamespaceResp
                                   .WatchAsync<V1Namespace, V1NamespaceList>(
                                       ex => logger.LogError(
                                           $"{nameof(WatchNamespaces)} - WatchAsync - {ex.Message}",
                                           ex),
                                       stoppingToken))
                {
                    string k8sNamespace = item.Name();
                    switch (type)
                    {
                        case WatchEventType.Added:
                            if (!watchVulnerabilityReportCrsTaskDict.ContainsKey(k8sNamespace))
                            {
                                using IServiceScope scope = services.CreateScope();
                                foreach (IKubernetesNamespaceAddedOrModifiedHandler handler in scope.ServiceProvider
                                             .GetServices<IKubernetesNamespaceAddedOrModifiedHandler>())
                                {
                                    await handler.Handle(k8sNamespace);
                                }

                                CreateWatchVulnerabilityReportCrsTask(k8sNamespace, stoppingToken);
                            }

                            break;
                        case WatchEventType.Deleted:
                            if (watchVulnerabilityReportCrsTaskDict.TryRemove(
                                    k8sNamespace,
                                    out TaskWithCts taskWithCts))
                            {
                                await taskWithCts.Cts.CancelAsync();
                                using IServiceScope scope = services.CreateScope();
                                foreach (IKubernetesNamespaceDeletedHandler handler in scope.ServiceProvider
                                             .GetServices<IKubernetesNamespaceDeletedHandler>())
                                {
                                    await handler.Handle(k8sNamespace);
                                }
                            }

                            break;
                        case WatchEventType.Modified:
                        case WatchEventType.Error:
                        case WatchEventType.Bookmark:
                        default:
                            // Ignore
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(WatchNamespaces)} - {ex.Message}", ex);
            }
        }
    }
}
