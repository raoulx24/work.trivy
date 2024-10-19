using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

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
    public async Task<IEnumerable<ConfigAuditReportDto>> Get([FromQuery] string? namespaceName)
    {
        return await configAuditReportService.GetConfigAuditReportDtos(namespaceName);
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
}
