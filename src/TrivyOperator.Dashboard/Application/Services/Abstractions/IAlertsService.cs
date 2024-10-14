using TrivyOperator.Dashboard.Application.Alerts;
using TrivyOperator.Dashboard.Application.Models;

namespace TrivyOperator.Dashboard.Application.Services.Abstractions;
public interface IAlertsService
{
    Task AddAlert(string emitter, Alert alert);
    Task<IList<AlertDto>> GetAllAlertDtos();
    Task RemoveAlert(string emitter, Alert alert);
}