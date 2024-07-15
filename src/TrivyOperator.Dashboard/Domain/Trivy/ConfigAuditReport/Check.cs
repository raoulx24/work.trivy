using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

namespace TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;

public class Check
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("checkID")]
    public string? CheckId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("messages")]
    public string[]? Messages { get; set; }

    [JsonPropertyName("remediation")]
    public string? Remediation { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
