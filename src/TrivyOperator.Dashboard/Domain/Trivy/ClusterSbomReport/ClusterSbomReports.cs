using k8s.Models;
using k8s;
using System.Text.Json.Serialization;
using TrivyOperator.Dashboard.Domain.Trivy.CustomResources.Abstractions;

namespace TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

public class Artifact
{
    [JsonPropertyName("repository")]
    public string Repository { get; init; } = string.Empty;

    [JsonPropertyName("tag")]
    public string Tag { get; init; } = string.Empty;
}

public class ClusterSbomReportCr : CustomResource, IKubernetesObject<V1ObjectMeta>
{
    [JsonPropertyName("report")]
    public Report? Report { get; init; }
}

public class ClusterSbomReportCrd : CustomResourceDefinition
{
    public override string Version { get; } = "v1alpha1";
    public override string Group { get; } = "aquasecurity.github.io";
    public override string PluralName { get; } = "clustersbomreports";
    public override string Kind { get; } = "CResource";
    public override string? Namespace { get; init; } = null;
}

public class Components
{
    [JsonPropertyName("bomFormat")]
    public string BomFormat { get; init; } = string.Empty;

    [JsonPropertyName("components")]
    public ComponentsComponent[] ComponentsComponents { get; init; } = [];

    [JsonPropertyName("dependencies")]
    public Dependency[] Dependencies { get; init; } = [];

    [JsonPropertyName("metadata")]
    public ComponentsMetadata Metadata { get; init; } = new();

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; init; } = string.Empty;

    [JsonPropertyName("specVersion")]
    public string SpecVersion { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public long Version { get; init; }
}

public class ComponentsComponent
{
    [JsonPropertyName("bom-ref")]
    public string BomRef { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("properties")]
    public Property[] Properties { get; init; } = [];

    [JsonPropertyName("purl")]
    public string Purl { get; init; } = string.Empty;

    //[JsonPropertyName("supplier")]
    //public Supplier Supplier { get; init; } = new();

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;
}

public class ComponentsMetadata
{
    [JsonPropertyName("component")]
    public ComponentsComponent Component { get; init; } = new();

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; init; }

    [JsonPropertyName("tools")]
    public Tools Tools { get; init; } = new();
}

public class Dependency
{
    [JsonPropertyName("dependsOn")]
    public string[] DependsOn { get; init; } = [];

    [JsonPropertyName("ref")]
    public string Ref { get; init; } = string.Empty;
}

public partial class Property
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}

public class Registry
{
    [JsonPropertyName("server")]
    public string Server { get; init; } = string.Empty;
}

public class Report
{
    [JsonPropertyName("artifact")]
    public Artifact Artifact { get; init; } = new();

    [JsonPropertyName("components")]
    public Components Components { get; init; } = new();

    [JsonPropertyName("registry")]
    public Registry Registry { get; init; } = new();

    [JsonPropertyName("scanner")]
    public Scanner Scanner { get; init; } = new();

    [JsonPropertyName("summary")]
    public Summary Summary { get; init; } = new();

    [JsonPropertyName("updateTimestamp")]
    public DateTime? UpdateTimestamp { get; init; }
}

public class Scanner
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("vendor")]
    public string Vendor { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;
}

public class Summary
{
    [JsonPropertyName("componentsCount")]
    public long ComponentsCount { get; init; } = 0;

    [JsonPropertyName("dependenciesCount")]
    public long DependenciesCount { get; init; } = 0;
}

public class Tools
{
    [JsonPropertyName("components")]
    public ToolsComponent[] Components { get; init; } = [];
}

public class ToolsComponent
{
    [JsonPropertyName("group")]
    public string Group { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    //[JsonPropertyName("supplier")]
    //public Supplier Supplier { get; init; } = new();

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;
}
