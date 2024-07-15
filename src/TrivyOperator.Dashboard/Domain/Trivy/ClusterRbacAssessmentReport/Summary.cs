using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

public class Summary
{
    [JsonPropertyName("criticalCount")]
    public long CriticalCount { get; set; }

    [JsonPropertyName("highCount")]
    public long HighCount { get; set; }

    [JsonPropertyName("lowCount")]
    public long LowCount { get; set; }

    [JsonPropertyName("mediumCount")]
    public long MediumCount { get; set; }
}
