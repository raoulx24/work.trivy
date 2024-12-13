using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.SbomReport;

public class Registry
{
    [JsonPropertyName("server")]
    public string Server { get; init; } = string.Empty;
}
