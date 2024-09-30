using TrivyOperator.Dashboard.Application.Services.WatcherState;

namespace TrivyOperator.Dashboard.Application.Models;

public class WatcherStateInfoDto
{
    public string KubernetesObjectType { get; init; } = string.Empty;
    public string? NamespaceName { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? LastException { get; init; }
}

public static class WatcherStateInfoExtensions
{
    public static WatcherStateInfoDto ToWatcherStateInfoDto(this WatcherStateInfo watcherStateInfo)
    {
        return new()
        {
            KubernetesObjectType = watcherStateInfo?.WatchedKubernetesObjectType.Name ?? string.Empty,
            NamespaceName = watcherStateInfo?.NamespaceName,
            Message = watcherStateInfo?.Message ?? string.Empty,
            LastException = watcherStateInfo?.LastException?.Message ?? string.Empty,
        };
    }
}