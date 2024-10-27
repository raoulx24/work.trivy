using Microsoft.Extensions.Options;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.Options;
using TrivyOperator.Dashboard.Infrastructure.Clients;

namespace TrivyOperator.Dashboard.Application.Services;

public class BackendSettingsService(IOptions<KubernetesOptions> options) : IBackendSettingsService
{
    public Task<BackendSettingsDto> GetBackendSettings()
    {
        List<string> backends = [];
        if (options.Value.TrivyUseClusterRbacAssessmentReport ?? false) backends.Add("crar");
        if (options.Value.TrivyUseConfigAuditReport ?? false) backends.Add("car");
        if (options.Value.TrivyUseExposedSecretReport ?? false) backends.Add("esr");
        if (options.Value.TrivyUseVulnerabilityReport ?? false) backends.Add("vr");

        return Task.FromResult<BackendSettingsDto>(new() { EnabledTrivyReports = backends });
    }
}
