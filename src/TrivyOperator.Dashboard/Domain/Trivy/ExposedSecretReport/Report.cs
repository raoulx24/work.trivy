using Microsoft.AspNetCore.DataProtection;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Report
{
    [JsonPropertyName("artifact")]
    public Artifact? Artifact { get; init; }

    [JsonPropertyName("registry")]
    public Registry? Registry { get; init; }

    [JsonPropertyName("scanner")]
    public Scanner? Scanner { get; init; }

    [JsonPropertyName("secrets")]
    public Secret[]? Secrets { get; init; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; init; }

    [JsonPropertyName("updateTimestamp")]
    public DateTime? UpdateTimestamp { get; init; }
}
