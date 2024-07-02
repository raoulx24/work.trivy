namespace TrivyOperator.Dashboard.Application.Services.Watchers;

public readonly struct TaskWithCts
{
    public Task Task { get; init; }
    public CancellationTokenSource Cts { get; init; }
}
