using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public abstract class KubernetesWatcher<TK8sObjectList, TKubernetesObject> : IKubernetesWatcher<TK8sObjectList, TKubernetesObject>
    where TKubernetesObject : IKubernetesObject
    where TK8sObjectList : IKubernetesObject, IItems<TKubernetesObject>


{
    protected ILogger logger;

    public async Task Watch(Kubernetes k8sClient, CancellationToken cancellationToken, string? k8sNamespace = null)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Task<HttpOperationResponse<TK8sObjectList>> listNamespaceResp = GetKubernetesObjectWatchList(k8sClient, k8sNamespace, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in listNamespaceResp
                                   .WatchAsync<TKubernetesObject, TK8sObjectList>(
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TK8sObjectList, TKubernetesObject>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    await ProcessWatchEvent(type, item, k8sNamespace, cancellationToken);
                }
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // TODO: something something
            }
            catch (HttpOperationException hoe) when (hoe.Response.StatusCode == HttpStatusCode.Forbidden)
            {
                // TODO: something something
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(KubernetesWatcher<TK8sObjectList, TKubernetesObject>)} - {ex.Message}", ex);
            }
        }
    }

    protected abstract Task<HttpOperationResponse<TK8sObjectList>> GetKubernetesObjectWatchList(Kubernetes k8sClient, string? k8sNamespace, CancellationToken cancellationToken);
    protected abstract Task ProcessWatchEvent(WatchEventType type, TKubernetesObject item, string? k8sNamespace, CancellationToken cancellationToken);
}
