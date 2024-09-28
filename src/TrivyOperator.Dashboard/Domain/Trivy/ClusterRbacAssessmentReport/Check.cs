using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

public class Check
{
    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;

    [JsonPropertyName("checkID")]
    public string CheckId { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("messages")]
    public string[] Messages { get; init; } = [];

    [JsonPropertyName("remediation")]
    public string Remediation { get; init; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; init; } = string.Empty;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
}
