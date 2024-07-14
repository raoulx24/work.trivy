namespace TrivyOperator.Dashboard.Application.Services.Options;

public record KubernetesOptions
{
    public string? KubeConfigFileName { get; init; }
    public string? NamespaceList { get; init; }
}
