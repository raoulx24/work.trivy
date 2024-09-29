namespace TrivyOperator.Dashboard.Application.Services.WatcherErrorHandlers;

public class WatcherStateDetails
{
    public string Message { get; set; } = string.Empty;
    public Exception? LastException { get; set; }
}
