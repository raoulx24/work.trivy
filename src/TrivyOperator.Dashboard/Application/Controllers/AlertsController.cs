using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController(IAlertsService alertsService)
    : ControllerBase
{
    [HttpGet(Name = "GetAlerts")]
    [ProducesResponseType<IEnumerable<AlertDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<AlertDto>> GetAll()
    {
        return await alertsService.GetAlertDtos();
    }
}
