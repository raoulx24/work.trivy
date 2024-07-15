using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Artifact
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }
}
