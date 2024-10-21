using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/config-audit-reports")]
public class ConfigAuditReportController(
    IConfigAuditReportService configAuditReportService,
    ILogger<ConfigAuditReportController> logger): ControllerBase
{
    [HttpGet(Name = "GetConfigAuditReportDtos")]
    [ProducesResponseType<IEnumerable<ConfigAuditReportDto>>(StatusCodes.Status200OK)]
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

        IEnumerable<ConfigAuditReportDto> configAuditReportImageDtos = await configAuditReportService.GetConfigAuditReportDtos(namespaceName, excludedSeverityIds);
        return Ok(configAuditReportImageDtos);
    }

    [HttpGet("denormalized", Name = "GetConfigAuditReportDenormalizedDto")]
    [ProducesResponseType<IEnumerable<ConfigAuditReportDenormalizedDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ConfigAuditReportDenormalizedDto>> GetDenormalized()
    {
        return await configAuditReportService.GetConfigAuditReportDenormalizedDtos();
    }

    [HttpGet("active-namespaces", Name = "GetConfigAuditReportActiveNamespaces")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<string>> GetActiveNamespaces()
    {
        return await configAuditReportService.GetActiveNamespaces();
    }

    [HttpGet("summary", Name = "GetConfigAuditReportSumaryDtos")]
    [ProducesResponseType<IEnumerable<ConfigAuditReportSummaryDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ConfigAuditReportSummaryDto>> GetConfigAuditReportSumaryDtos()
    {
        return await configAuditReportService.GetConfigAuditReportSummaryDtos();
    }
}
