using System.Collections.Concurrent;
using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services;

public readonly struct TaskWithCancellationToken
{
    public Task Task { get; init; }
    public CancellationTokenSource Cts { get; init; }
}

public class KubernetesHostedService(
    IServiceProvider services,
    IK8sClientFactory k8sClientFactory,
    ILogger<KubernetesHostedService> logger)
    : BackgroundService
{
    private readonly ConcurrentDictionary<string, TaskWithCancellationToken> watchDict = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Hosted Service running.");
        Kubernetes k8sClient = k8sClientFactory.GetClient();
        V1NamespaceList nsList = await k8sClient.CoreV1.ListNamespaceAsync();
        foreach (V1Namespace ns in nsList.Items)
        {
            string k8sNamespace = ns.Name();
            CancellationTokenSource cts = new();
            TaskWithCancellationToken taskWithCancellationToken = new()
            {
                Task = WatchPods(k8sClient, k8sNamespace,
                    CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, cts.Token).Token),
                Cts = cts
            };
            watchDict[k8sNamespace] = taskWithCancellationToken;
        }
        // TODO: watch namespaces
        // create watch pods: watchDict["default"] = taskWithCancellationToken;
        // cancel watch pods: await watchDict["default"].Cts.CancelAsync();
    }

    private async Task WatchPods(Kubernetes k8sClient, string k8sNamespace, CancellationToken stoppingToken)
    {
        Task<HttpOperationResponse<V1PodList>>? listNamespacedPodResp =
            k8sClient.CoreV1.ListNamespacedPodWithHttpMessagesAsync(k8sNamespace, watch: true,
                cancellationToken: stoppingToken);
        await foreach ((WatchEventType type, V1Pod? item) in listNamespacedPodResp.WatchAsync<V1Pod, V1PodList>(
                           cancellationToken: stoppingToken))
        {
            using IServiceScope scope = services.CreateScope();
            foreach (IKubernetesPodWatchEventHandler handler in scope.ServiceProvider
                         .GetServices<IKubernetesPodWatchEventHandler>())
            {
                await handler.Handle(type, item);
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kubernetes Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
