using Microsoft.Extensions.Options;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;

namespace TrivyOperator.Dashboard.Application.Services;

public class BackendSettingsService(IOptions<KubernetesOptions> options) : IBackendSettingsService
{
    public Task<BackendSettingsDto> GetBackendSettings()
    {
        BackendSettingsDto backendSettingsDto = new()
        {
            TrivyReportConfigDtos = [] 
        };

        backendSettingsDto.TrivyReportConfigDtos.Add(new()
        {
            Id = "crar",
            Enabled = options.Value.TrivyUseClusterRbacAssessmentReport ?? false,
        });
        backendSettingsDto.TrivyReportConfigDtos.Add(new()
        {
            Id = "car",
            Enabled = options.Value.TrivyUseConfigAuditReport ?? false,
        });
        backendSettingsDto.TrivyReportConfigDtos.Add(new()
        {
            Id = "esr",
            Enabled = options.Value.TrivyUseExposedSecretReport ?? false,
        });
        backendSettingsDto.TrivyReportConfigDtos.Add(new()
        {
            Id = "vr",
            Enabled = options.Value.TrivyUseVulnerabilityReport ?? false,
        });

        return Task.FromResult<BackendSettingsDto>(backendSettingsDto);
    }
}
