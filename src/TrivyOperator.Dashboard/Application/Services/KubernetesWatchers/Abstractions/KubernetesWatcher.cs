using k8s;
using k8s.Autorest;
using k8s.Models;
using System.Net;
using TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.KubernetesWatchers.Abstractions;

public abstract class KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams> :
    IKubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>
        where TKubernetesObject : IKubernetesObject
        where TKubernetesObjectList : IKubernetesObject, IItems<TKubernetesObject>
        where TWatcherParams : IKubernetesWatcherParams
{
    protected ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>> logger;
    protected Dictionary<string, TaskWithCts> watchers = [];
    protected Kubernetes kubernetesClient;

    public KubernetesWatcher(IKubernetesClientFactory kubernetesClientFactory,
        ILogger<KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>> logger)
    {
        kubernetesClient = kubernetesClientFactory.GetClient();
        this.logger = logger;
    }

    public void Add(TWatcherParams watcherParams)
    {
        CancellationTokenSource cts = new();
        TaskWithCts taskWithCts = new()
        {
            Task = CreateWatch(
                        watcherParams,
                        CancellationTokenSource.CreateLinkedTokenSource(watcherParams.CancellationToken, cts.Token).Token),
            Cts = cts,
        };
        TaskWithCts watcherWithCts = taskWithCts;

        AddWatcherWithCtsToWatchers(watcherWithCts, watcherParams);
    }

    protected async Task CreateWatch(TWatcherParams watcherParams, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Task<HttpOperationResponse<TKubernetesObjectList>> listNamespaceResp = GetKubernetesObjectWatchList(watcherParams, cancellationToken);
                await foreach ((WatchEventType type, TKubernetesObject item) in listNamespaceResp
                                   .WatchAsync<TKubernetesObject, TKubernetesObjectList>(
                                       ex => logger.LogError(
                                           $"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>)} - WatchAsync - {ex.Message}",
                                           ex),
                                       cancellationToken))
                {
                    // TODO: implement Channel
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
                logger.LogError($"{nameof(KubernetesWatcher<TKubernetesObjectList, TKubernetesObject, TWatcherParams>)} - {ex.Message}", ex);
            }
        }
    }

    protected abstract Task<HttpOperationResponse<TKubernetesObjectList>> GetKubernetesObjectWatchList(TWatcherParams watcherParams, CancellationToken cancellationToken);
    protected abstract void AddWatcherWithCtsToWatchers(TaskWithCts watcher, TWatcherParams watcherParams);
}
