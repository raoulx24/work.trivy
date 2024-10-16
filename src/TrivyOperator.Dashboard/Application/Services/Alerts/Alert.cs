namespace TrivyOperator.Dashboard.Application.Services.Alerts;

public class Alert
{
    public string EmitterKey { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Severity Severity { get; init; } = Severity.Info;
}
