namespace TrivyOperator.Dashboard.Application.Services.WatcherState;

public class WatcherStateInfo
{
    public required Type WatchedKubernetesObjectType { get; set; }
    public string? NamespaceName {get; set; } 
    public string Message { get; set; } = string.Empty;
    public Exception? LastException { get; set; }
}
