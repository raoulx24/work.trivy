using k8s.Autorest;

namespace TrivyOperator.Dashboard.Application.Services.WatcherErrorHandlers;
public interface IWatcherState
{
    Task ProcessWatcherError(Type watchedKubernetesObjectType, string watcherKey, HttpOperationException exception);
    ValueTask ProcessWatcherSuccess(Type watchedKubernetesObjectType, string watcherKey);
    ValueTask ProcessWatcherCancel(Type watchedKubernetesObjectType, string watcherKey);
}