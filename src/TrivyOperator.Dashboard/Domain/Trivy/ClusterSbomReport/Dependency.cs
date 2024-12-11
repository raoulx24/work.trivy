﻿using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

public class Dependency
{
    [JsonPropertyName("dependsOn")]
    public string[] DependsOn { get; init; } = [];

    [JsonPropertyName("ref")]
    public string Ref { get; init; } = string.Empty;
}
