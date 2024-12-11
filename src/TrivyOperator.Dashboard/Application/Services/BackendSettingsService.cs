﻿using Microsoft.Extensions.Options;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;

namespace TrivyOperator.Dashboard.Application.Services;

public class BackendSettingsService(IOptions<KubernetesOptions> options) : IBackendSettingsService
{
    public Task<BackendSettingsDto> GetBackendSettings()
    {
        BackendSettingsDto backendSettingsDto = new() { TrivyReportConfigDtos = [] };

        backendSettingsDto.TrivyReportConfigDtos.Add(
            new BackendSettingsTrivyReportConfigDto
            {
                Id = "ccr",
                Name = "Cluster Compliance Report",
                Enabled = options.Value.TrivyUseVulnerabilityReport ?? false,
            });
        backendSettingsDto.TrivyReportConfigDtos.Add(
            new BackendSettingsTrivyReportConfigDto
            {
                Id = "crar",
                Name = "Cluster RBAC Assessment Report",
                Enabled = options.Value.TrivyUseClusterRbacAssessmentReport ?? false,
            });
        backendSettingsDto.TrivyReportConfigDtos.Add(
            new BackendSettingsTrivyReportConfigDto
            {
                Id = "car",
                Name = "Config Audit Report",
                Enabled = options.Value.TrivyUseConfigAuditReport ?? false,
            });
        backendSettingsDto.TrivyReportConfigDtos.Add(
            new BackendSettingsTrivyReportConfigDto
            {
                Id = "esr",
                Name = "Exposed Secret Report",
                Enabled = options.Value.TrivyUseExposedSecretReport ?? false,
            });
        backendSettingsDto.TrivyReportConfigDtos.Add(
            new BackendSettingsTrivyReportConfigDto
            {
                Id = "vr",
                Name = "Vulnerability Report",
                Enabled = options.Value.TrivyUseVulnerabilityReport ?? false,
            });

        return Task.FromResult(backendSettingsDto);
    }
}
