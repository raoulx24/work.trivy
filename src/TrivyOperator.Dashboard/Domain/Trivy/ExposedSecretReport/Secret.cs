using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Secret
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("match")]
    public string? Match { get; set; }

    [JsonPropertyName("ruleID")]
    public string? RuleId { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
