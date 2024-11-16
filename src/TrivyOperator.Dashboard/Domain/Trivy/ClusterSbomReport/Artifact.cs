using System.Text.Json.Serialization;
using YamlDotNet.Core.Tokens;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

public class Artifact
{
    [JsonPropertyName("repository")]
    public string Repository { get; init; } = string.Empty;

    [JsonPropertyName("tag")]
    public string Tag { get; init; } = string.Empty;
}
