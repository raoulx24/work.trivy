using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IBackendSettingsService
{
    Task<BackendSettingsDto> GetBackendSettings();
}