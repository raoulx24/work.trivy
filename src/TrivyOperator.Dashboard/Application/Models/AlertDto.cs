using TrivyOperator.Dashboard.Application.Services.Alerts;

namespace TrivyOperator.Dashboard.Application.Models;

public class AlertDto
{
    public string Emiter { get; init; } = string.Empty;
    public string EmitterKey { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Severity Severity { get; init; } = Severity.Info;
}

public static class AlertExtensions
{
    public static AlertDto ToAlertDto(this Alert alert, string emitter) => new()
    {
        Emiter = emitter,
        EmitterKey = alert.EmitterKey,
        Message = alert.Message,
        Severity = alert.Severity,
    };
}
