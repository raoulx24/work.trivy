using k8s.Autorest;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Alerts;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.WatcherStates;

public class WatcherState(
    IConcurrentCache<string, WatcherStateInfo> watcherStateCache,
    IAlertsService alertService,
    ILogger<WatcherState> logger) : IWatcherState
{
    private readonly string alertEmitter = "Watcher";

    public async Task ProcessWatcherError(
        Type watchedKubernetesObjectType,
        string watcherKey,
        HttpOperationException exception)
    {
        logger.LogError(
            "Watcher for {kubernetesObjectType} and key {watcherKey} crashed with {httpError}",
            watchedKubernetesObjectType.Name,
            watcherKey,
            (int)exception.Response.StatusCode);
        await AddOrUpdateKey(watchedKubernetesObjectType, watcherKey, exception);

        await Task.Delay(60000);
    }

    public async ValueTask ProcessWatcherSuccess(Type watchedKubernetesObjectType, string watcherKey) =>
        await AddOrUpdateKey(watchedKubernetesObjectType, watcherKey);

    public ValueTask ProcessWatcherCancel(Type watchedKubernetesObjectType, string watcherKey)
    {
        logger.LogInformation(
            "Watcher for {kubernetesObjectType} and key {watcherKey} was canceled.",
            watchedKubernetesObjectType,
            watcherKey);
        watcherStateCache.TryRemove(GetCacheKey(watchedKubernetesObjectType, watcherKey), out _);
        return ValueTask.CompletedTask;
    }

    private static string GetCacheKey(Type watchedKubernetesObjectType, string watcherKey) =>
        $"{watchedKubernetesObjectType.Name}|{watcherKey}";

    private async ValueTask AddOrUpdateKey(
        Type watchedKubernetesObjectType,
        string watcherKey,
        Exception? newException = null)
    {
        string cacheKey = GetCacheKey(watchedKubernetesObjectType, watcherKey);
        watcherStateCache.TryGetValue(cacheKey, out WatcherStateInfo? watcherStateDetails);
        if (watcherStateDetails == null)
        {
            WatcherStateInfo newWatcherStateDetails = new()
            {
                WatchedKubernetesObjectType = watchedKubernetesObjectType,
                NamespaceName = watcherKey == VarUtils.DefaultCacheRefreshKey ? null : watcherKey,
                Status = newException == null ? WatcherStateStatus.Green : WatcherStateStatus.Red,
                LastException = newException,
            };
            watcherStateCache.TryAdd(cacheKey, newWatcherStateDetails);
        }
        else
        {
            watcherStateDetails.Status = newException == null ? WatcherStateStatus.Green : WatcherStateStatus.Red;
            watcherStateDetails.LastException = newException ?? watcherStateDetails.LastException;
        }

        await SetAlert(watchedKubernetesObjectType, watcherKey, newException);
    }

    private async Task SetAlert(Type watchedKubernetesObjectType, string watcherKey, Exception? newException = null)
    {
        if (newException != null)
        {
            string namespaceName = watcherKey == VarUtils.DefaultCacheRefreshKey ? "n/a" : watcherKey;
            await alertService.AddAlert(
                alertEmitter,
                new Alert
                {
                    EmitterKey = GetCacheKey(watchedKubernetesObjectType, watcherKey),
                    Message =
                        $"Watcher for {watchedKubernetesObjectType.Name} and Namespace {namespaceName} failed.",
                    Severity = Severity.Error,
                });
        }
        else
        {
            await alertService.RemoveAlert(
                alertEmitter,
                new Alert
                {
                    EmitterKey = GetCacheKey(watchedKubernetesObjectType, watcherKey),
                    Message = string.Empty,
                    Severity = Severity.Info,
                });
        }
    }
}
