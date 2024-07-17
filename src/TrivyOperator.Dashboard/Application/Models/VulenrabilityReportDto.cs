﻿using TrivyOperator.Dashboard.Domain.Trivy.VulnerabilityReport;

namespace TrivyOperator.Dashboard.Application.Models;

public class VulnerabilityReportDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public string? ResourceContainerName { get; init; }
    public string? ImageName { get; init; }
    public string? ImageTag { get; init; }
    public string? ImageRepository { get; init; }
    public string? ImageOsFamily { get; init; }
    public string? ImageOsName { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }
    public VulnerabilityReportDetailDto[]? Vulnerabilities { get; init; }
}

public class VulnerabilityReportDetailDto
{
    public string? FixedVersion { get; init; }
    public string? InstalledVersion { get; init; }
    public string? LastModifiedDate {  get; init; }
    public Uri? PrimaryLink { get; init; }
    public string? PublishedDate { get; set; }
    public string? Resource { get; set; }
    public double Score { get; set; }
    public string? Severity { get; set; }
    public string? Target { get; set; }
    public string? Title { get; set; }
    public string? VulnerabilityId { get; set; }
}

public class VulnerabilityReportDenormalizedDto
{
    public Guid Uid { get; init; }
    public string? ResourceName { get; init; }
    public string? ResourceNamespace { get; init; }
    public string? ResourceKind { get; init; }
    public string? ResourceContainerName { get; init; }
    public string? ImageName { get; init; }
    public string? ImageTag { get; init; }
    public string? ImageRepository { get; init; }
    public string? ImageOsFamily { get; init; }
    public string? ImageOsName { get; init; }
    public long CriticalCount { get; init; }
    public long HighCount { get; init; }
    public long MediumCount { get; init; }
    public long LowCount { get; init; }

    public string? FixedVersion { get; init; }
    public string? InstalledVersion { get; init; }
    public string? LastModifiedDate { get; init; }
    public Uri? PrimaryLink { get; init; }
    public string? PublishedDate { get; set; }
    public string? Resource { get; set; }
    public double Score { get; set; }
    public string? Severity { get; set; }
    public string? Target { get; set; }
    public string? Title { get; set; }
    public string? VulnerabilityId { get; set; }
}

public static class VulenrabilityReportCrExtensions
{
    public static VulnerabilityReportDto ToVulnerabilityReportDto(this VulnerabilityReportCr vulnerabilityReportCR)
    {
        List<VulnerabilityReportDetailDto> vulenrabilityReportDetailDtos = new();
        foreach (Vulnerability vulnerability in vulnerabilityReportCR.Report.Vulnerabilities)
        {
            VulnerabilityReportDetailDto vulnerabilityReportDetailDto = new()
            {
                FixedVersion = vulnerability.FixedVersion,
                InstalledVersion = vulnerability.InstalledVersion,
                LastModifiedDate = vulnerability.LastModifiedDate,
                PrimaryLink = vulnerability.PrimaryLink,
                PublishedDate = vulnerability.PublishedDate,
                Resource = vulnerability.Resource,
                Score = vulnerability.Score,
                Severity = vulnerability.Severity,
                Target = vulnerability.Target,
                Title = vulnerability.Title,
                VulnerabilityId = vulnerability.VulnerabilityId,
            };
            vulenrabilityReportDetailDtos.Add(vulnerabilityReportDetailDto);
        }
        VulnerabilityReportDto vulnerabilityReportDto = new()
        {
            Uid = new Guid(vulnerabilityReportCR.Metadata.Uid),
            ResourceName = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
            ResourceNamespace = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
            ResourceKind = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
            ResourceContainerName = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.container.name")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.container.name"]
                : string.Empty,
            ImageName = vulnerabilityReportCR.Report?.Artifact?.Repository,
            ImageTag = vulnerabilityReportCR.Report?.Artifact?.Tag,
            ImageRepository = vulnerabilityReportCR.Report?.Registry?.Server,
            ImageOsFamily = vulnerabilityReportCR.Report?.Os?.Family,
            ImageOsName = vulnerabilityReportCR.Report?.Os?.Name,
            CriticalCount = vulnerabilityReportCR.Report?.Summary?.CriticalCount ?? 0,
            HighCount = vulnerabilityReportCR.Report?.Summary?.HighCount ?? 0,
            MediumCount = vulnerabilityReportCR.Report?.Summary?.MediumCount ?? 0,
            LowCount = vulnerabilityReportCR.Report?.Summary?.LowCount ?? 0,
            Vulnerabilities = [.. vulenrabilityReportDetailDtos],
        };

        return vulnerabilityReportDto;
    }

    public static IList<VulnerabilityReportDenormalizedDto> ToVulnerabilityReportDenormalizedDtos(this VulnerabilityReportCr vulnerabilityReportCR)
    {
        List<VulnerabilityReportDenormalizedDto> vulnerabilityReportDenormalizedDtos = [];
        foreach (Vulnerability vulnerability in vulnerabilityReportCR.Report.Vulnerabilities)
        {
            VulnerabilityReportDenormalizedDto vulnerabilityReportDenormalizedDto = new()
            {
                FixedVersion = vulnerability.FixedVersion,
                InstalledVersion = vulnerability.InstalledVersion,
                LastModifiedDate = vulnerability.LastModifiedDate,
                PrimaryLink = vulnerability.PrimaryLink,
                PublishedDate = vulnerability.PublishedDate,
                Resource = vulnerability.Resource,
                Score = vulnerability.Score,
                Severity = vulnerability.Severity,
                Target = vulnerability.Target,
                Title = vulnerability.Title,
                VulnerabilityId = vulnerability.VulnerabilityId,

                Uid = new Guid(vulnerabilityReportCR.Metadata.Uid),
                ResourceName = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.name")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.name"]
                : string.Empty,
                ResourceNamespace = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.namespace")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.namespace"]
                : string.Empty,
                ResourceKind = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.resource.kind")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.resource.kind"]
                : string.Empty,
                ResourceContainerName = vulnerabilityReportCR.Metadata.Labels.ContainsKey("trivy-operator.container.name")
                ? vulnerabilityReportCR.Metadata.Labels["trivy-operator.container.name"]
                : string.Empty,
                ImageName = vulnerabilityReportCR.Report?.Artifact?.Repository,
                ImageTag = vulnerabilityReportCR.Report?.Artifact?.Tag,
                ImageRepository = vulnerabilityReportCR.Report?.Registry?.Server,
                ImageOsFamily = vulnerabilityReportCR.Report?.Os?.Family,
                ImageOsName = vulnerabilityReportCR.Report?.Os?.Name,
                CriticalCount = vulnerabilityReportCR.Report?.Summary?.CriticalCount ?? 0,
                HighCount = vulnerabilityReportCR.Report?.Summary?.HighCount ?? 0,
                MediumCount = vulnerabilityReportCR.Report?.Summary?.MediumCount ?? 0,
                LowCount = vulnerabilityReportCR.Report?.Summary?.LowCount ?? 0,
            };
            vulnerabilityReportDenormalizedDtos.Add(vulnerabilityReportDenormalizedDto);
        }

        return vulnerabilityReportDenormalizedDtos;
    }
}