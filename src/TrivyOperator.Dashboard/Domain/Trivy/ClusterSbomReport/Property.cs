using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

public partial class Property
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}
