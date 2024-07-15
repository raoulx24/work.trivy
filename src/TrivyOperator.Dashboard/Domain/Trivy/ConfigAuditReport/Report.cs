using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

namespace TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;

public class Report
{
    [JsonPropertyName("checks")]
    public Check[] Checks { get; set; } = new Check[0];

    [JsonPropertyName("scanner")]
    public Scanner? Scanner { get; set; }

    [JsonPropertyName("summary")]
    public Summary? Summary { get; set; }

    [JsonPropertyName("updateTimestamp")]
    public string? UpdateTimestamp { get; set; }
}
