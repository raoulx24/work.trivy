using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Application.Services.WatcherStates;
using TrivyOperator.Dashboard.Domain.Trivy;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/watcher-state-infos")]
public class WatcherStateInfoController(IWatcherStateInfoService watcherStateInfoService, ILogger<WatcherStateInfoController> logger)
    : ControllerBase
{
    [HttpGet(Name = "get WatcherStateInfos")]
    [ProducesResponseType<IEnumerable<WatcherStateInfoDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<WatcherStateInfoDto>> GetAll()
    {
        return await watcherStateInfoService.GetWatcherStateInfos();
    }
}
