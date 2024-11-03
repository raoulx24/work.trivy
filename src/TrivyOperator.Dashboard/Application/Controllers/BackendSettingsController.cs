using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/backend-settings")]
public class BackendSettingsController(IBackendSettingsService service) : ControllerBase
{
    [HttpGet(Name = "getBackendSettings")]
    [ProducesResponseType<BackendSettingsDto>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<BackendSettingsDto> GetBackendSettings() => await service.GetBackendSettings();
}
