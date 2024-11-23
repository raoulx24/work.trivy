﻿using TrivyOperator.Dashboard.Domain.Trivy.ClusterSbomReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class ClusterSbomReportDto
{
    public string Uid { get; set; } = Guid.NewGuid().ToString();
    public string ImageName { get; set; } = string.Empty;
    public string ImageTag { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public ClusterSbomReportDetailDto[] Details { get; set; } = [];
}

public class ClusterSbomReportDetailDto
{
    public Guid BomRef { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Purl { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Guid[] DependsOn { get; set; } = [];
}

public static class ClusterSbomReportCrExtensions
{
    public static ClusterSbomReportDto ToClusterSbomReportDto(this ClusterSbomReportCr clusterSbomReportCr)
    {
        var allComponents = clusterSbomReportCr.Report?.Components.ComponentsComponents ?? [];
        Array.Resize(ref allComponents, allComponents.Length + 1);
        allComponents[^1] = new()
        {
            BomRef = clusterSbomReportCr.Report?.Components.Metadata.Component.BomRef ?? Guid.Empty.ToString(),
            Name = clusterSbomReportCr.Report?.Components.Metadata.Component.Name ?? string.Empty,
            Purl = clusterSbomReportCr.Report?.Components.Metadata.Component.Purl ?? Guid.Empty.ToString(),
            Type = clusterSbomReportCr.Report?.Components.Metadata.Component.Type ?? Guid.Empty.ToString(),
            Version = clusterSbomReportCr.Report?.Components.Metadata.Component.Version ?? Guid.Empty.ToString(),
        };
        var alldependencies = clusterSbomReportCr.Report?.Components.Dependencies ?? [];

        var details = allComponents.Select(component =>
        {
            Guid.TryParse(component.BomRef, out Guid bomRef);

            var refDependency = alldependencies.FirstOrDefault(dep => dep.Ref == bomRef.ToString() || dep.Ref == component.Purl) ?? new();
            var dependencies = refDependency.DependsOn
                .Select(depOn => {
                    var dependsOn = allComponents.FirstOrDefault(dep => dep.BomRef == depOn || dep.Purl == depOn)?.BomRef ?? string.Empty;
                    Guid.TryParse(dependsOn, out Guid dependsOnBomRef);

                    return dependsOnBomRef;
                }).ToArray();
            ClusterSbomReportDetailDto detailDto = new()
            {
                BomRef = bomRef,
                Name = component.Name,
                Purl = component.Purl,
                Version = component.Version,
                DependsOn = dependencies,
            };

            return detailDto;
        });

        ClusterSbomReportDto result = new()
        {
            Uid = clusterSbomReportCr.Metadata.Uid,
            ImageName = clusterSbomReportCr.Report?.Artifact?.Repository ?? string.Empty,
            ImageTag = clusterSbomReportCr.Report?.Artifact?.Tag ?? string.Empty,
            Repository = clusterSbomReportCr.Report?.Registry?.Server ?? string.Empty,
            Details = details.ToArray(),
        };

        return result;
    }
}