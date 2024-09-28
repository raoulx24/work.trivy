using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterRbacAssessmentReport;

public class Scanner
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("vendor")]
    public string Vendor { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;
}
