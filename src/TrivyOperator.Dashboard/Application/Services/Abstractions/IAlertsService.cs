using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Alerts;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;

public interface IAlertsService
{
    Task AddAlert(string emitter, Alert alert);
    Task<IList<AlertDto>> GetAlertDtos();
    Task RemoveAlert(string emitter, Alert alert);
}
