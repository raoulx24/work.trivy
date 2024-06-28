using k8s;
using k8s.Autorest;
using k8s.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public class NamespaceWatcher : KubernetesWatcher<V1NamespaceList, V1Namespace>
{
    public NamespaceWatcher(ILogger<NamespaceWatcher> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpOperationResponse<V1NamespaceList>> GetKubernetesObjectWatchList(Kubernetes k8sClient, string? k8sNamespace, CancellationToken cancellationToken)
    {
        return await k8sClient.CoreV1.ListNamespaceWithHttpMessagesAsync(
                        watch: true,
                        timeoutSeconds: int.MaxValue,
                        cancellationToken: cancellationToken);
    }

    protected override Task ProcessWatchEvent(WatchEventType type, V1Namespace item, string? k8sNamespace, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
