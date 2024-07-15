using Microsoft.AspNetCore.DataProtection;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Report
{
    [JsonPropertyName("artifact")]
    public Artifact? Artifact { get; set; }

    [JsonPropertyName("registry")]
    public Registry? Registry { get; set; }

    [JsonPropertyName("scanner")]
    public Scanner? Scanner { get; set; }

    [JsonPropertyName("secrets")]
    public Secret[]? Secrets { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }

    [JsonPropertyName("updateTimestamp")]
    public string? UpdateTimestamp { get; set; }
}
