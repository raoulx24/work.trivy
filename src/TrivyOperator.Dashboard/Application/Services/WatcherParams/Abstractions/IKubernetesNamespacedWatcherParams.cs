namespace TrivyOperator.Dashboard.Application.Services.WatcherParams.Abstractions;

public interface IKubernetesNamespacedWatcherParams : IKubernetesWatcherParams
{
    string kubernetesNamespace { get; init; }
}