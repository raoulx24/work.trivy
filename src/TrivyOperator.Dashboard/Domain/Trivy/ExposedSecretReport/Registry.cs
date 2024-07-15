using Microsoft.AspNetCore.Hosting.Server;
using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ExposedSecretReport;

public class Registry
{
    [JsonPropertyName("server")]
    public string? Server { get; set; }
}
