using System.Net;
using k8s.Autorest;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Services.WatcherStates;

public class WatcherState(IConcurrentCache<string, WatcherStateInfo> watcherStateCache, ILogger<WatcherState> logger)
    : IWatcherState
{
    public async Task ProcessWatcherError(Type watchedKubernetesObjectType, string watcherKey, HttpOperationException exception)
    {
        logger.LogError(
            "Watcher for {kubernetesObjectType} and key {watcherKey} crashed with {httpError}",
            watchedKubernetesObjectType.Name,
            watcherKey,
            (int)exception.Response.StatusCode);
        AddOrUpdateKey(watchedKubernetesObjectType, watcherKey, exception);

        await Task.Delay(60000);
    }

    public ValueTask ProcessWatcherSuccess(Type watchedKubernetesObjectType, string watcherKey)
    {
        AddOrUpdateKey(watchedKubernetesObjectType, watcherKey);
        return ValueTask.CompletedTask;
    }

    public ValueTask ProcessWatcherCancel(Type watchedKubernetesObjectType, string watcherKey)
    {
        logger.LogInformation("Watcher for {kubernetesObjectType} and key {watcherKey} was canceled.",
                    watchedKubernetesObjectType,
                    watcherKey);
        watcherStateCache.TryRemove(GetCacheKey(watchedKubernetesObjectType, watcherKey), out _);
        return ValueTask.CompletedTask; 
    }

    private static string GetCacheKey(Type watchedKubernetesObjectType, string watcherKey)
        => $"{watchedKubernetesObjectType.Name}|{watcherKey}";

    private void AddOrUpdateKey(Type watchedKubernetesObjectType, string watcherKey, Exception? newException = null)
    {
        string cacheKey = GetCacheKey(watchedKubernetesObjectType, watcherKey);
        watcherStateCache.TryGetValue(cacheKey, value: out WatcherStateInfo? watcherStateDetails);
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
    }
}
