using System.Net;
using k8s.Autorest;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Application.Services.WatcherErrorHandlers;

public class WatcherState(IConcurrentCache<string, WatcherStateDetails> watcherStateCache, ILogger<WatcherState> logger)
    : IWatcherState
{
    public async Task ProcessWatcherError(Type watchedKubernetesObjectType, string watcherKey, HttpOperationException exception)
    {
        switch (exception.Response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                logger.LogError(
                    "Watcher for {kubernetesObjectType} and key {watcherKey} crashed with 404.",
                    watchedKubernetesObjectType.Name,
                    watcherKey);
                break;
            case HttpStatusCode.Forbidden:
                logger.LogError(
                    "Watcher for {kubernetesObjectType} and key {watcherKey} crashed with 403.",
                    watchedKubernetesObjectType.Name,
                    watcherKey);
                break;
            default:
                logger.LogError(
                    "Watcher for {kubernetesObjectType} and key {watcherKey} crashed with {httpError}",
                    watchedKubernetesObjectType.Name,
                    watcherKey,
                    (int)exception.Response.StatusCode);
                break;
        }

        AddOrUpdateKey(watchedKubernetesObjectType, watcherKey, "Red", exception);

        await Task.Delay(60000);
    }

    public ValueTask ProcessWatcherSuccess(Type watchedKubernetesObjectType, string watcherKey)
    {
        AddOrUpdateKey(watchedKubernetesObjectType, watcherKey, "Green");
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

    private void AddOrUpdateKey(Type watchedKubernetesObjectType, string watcherKey, string newMessage, Exception? newException = null)
    {
        string cacheKey = GetCacheKey(watchedKubernetesObjectType, watcherKey);
        watcherStateCache.TryGetValue(cacheKey, value: out WatcherStateDetails? watcherStateDetails);
        if (watcherStateDetails == null)
        {
            WatcherStateDetails newWatcherStateDetails = new()
            {
                Message = newMessage,
                LastException = newException,
            };
            watcherStateCache.TryAdd(cacheKey, newWatcherStateDetails);
        }
        else
        {
            watcherStateDetails.Message = newMessage;
            watcherStateDetails.LastException = newException;
        }
    }
}
