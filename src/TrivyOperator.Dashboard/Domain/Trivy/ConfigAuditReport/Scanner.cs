using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TrivyOperator.Dashboard.Domain.Trivy.ConfigAuditReport;

public class Scanner
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("vendor")]
    public string? Vendor { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
