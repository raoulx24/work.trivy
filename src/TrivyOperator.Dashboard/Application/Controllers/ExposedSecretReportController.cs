using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/exposed-secret-reports")]
public class ExposedSecretReportController(
    IExposedSecretReportService exposedSecretReportService,
    ILogger<ExposedSecretReportController> logger): ControllerBase
{
    [HttpGet(Name = "GetExposedSecretReportDtos")]
    [ProducesResponseType<IEnumerable<ExposedSecretReportDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] string? namespaceName, [FromQuery] string? excludedSeverities)
    {
        List<int>? excludedSeverityIds = VarUtils.GetExcludedSeverityIdsFromStringList(excludedSeverities);

        if (excludedSeverityIds == null)
        {
            return BadRequest();
        }

        IEnumerable<ExposedSecretReportDto> ExposedSecretReportImageDtos = await exposedSecretReportService.GetExposedSecretReportDtos(namespaceName, excludedSeverityIds);
        return Ok(ExposedSecretReportImageDtos);
    }

    [HttpGet("denormalized", Name = "GetExposedSecretReportDenormalizedDto")]
    [ProducesResponseType<IEnumerable<ExposedSecretReportDenormalizedDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ExposedSecretReportDenormalizedDto>> GetDenormalized()
    {
        return await exposedSecretReportService.GetExposedSecretDenormalizedDtos();
    }

    [HttpGet("active-namespaces", Name = "GetExposedSecretReportActiveNamespaces")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<string>> GetActiveNamespaces()
    {
        return await exposedSecretReportService.GetActiveNamespaces();
    }

    [HttpGet("grouped-by-image", Name = "GetExposedSecretReportImageDtos")]
    [ProducesResponseType<IEnumerable<ExposedSecretReportImageDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupedByImage([FromQuery] string? namespaceName, [FromQuery] string? excludedSeverities)
    {
        List<int>? excludedSeverityIds = VarUtils.GetExcludedSeverityIdsFromStringList(excludedSeverities);

        if (excludedSeverityIds == null)
        {
            return BadRequest();
        }

        IEnumerable<ExposedSecretReportImageDto> exposedSecretReportImageDtos = await exposedSecretReportService.GetExposedSecretReportImageDtos(namespaceName, excludedSeverityIds);

        return Ok(exposedSecretReportImageDtos);
    }
}
