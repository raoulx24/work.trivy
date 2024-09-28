using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Artifact
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("digest")]
    public string Digest { get; init; } = string.Empty;

    [JsonPropertyName("repository")]
    public string Repository { get; init; } = string.Empty;

    [JsonPropertyName("tag")]
    public string Tag { get; init; } = string.Empty;
}
