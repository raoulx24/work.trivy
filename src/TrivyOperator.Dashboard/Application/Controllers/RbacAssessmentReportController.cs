using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/rbac-assessment-report")]
public class RbacAssessmentReportController(IRbacAssessmentReportService RbacAssessmentReportService) : ControllerBase
{
    [HttpGet(Name = "GetRbacAssessmentReportDtos")]
    [ProducesResponseType<IEnumerable<RbacAssessmentReportDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] string? namespaceName, [FromQuery] string? excludedSeverities)
    {
        List<int>? excludedSeverityIds = VarUtils.GetExcludedSeverityIdsFromStringList(excludedSeverities);

        if (excludedSeverityIds == null)
        {
            return BadRequest();
        }

        IEnumerable<RbacAssessmentReportDto> RbacAssessmentReportImageDtos =
            await RbacAssessmentReportService.GetRbacAssessmentReportDtos(namespaceName, excludedSeverityIds);
        return Ok(RbacAssessmentReportImageDtos);
    }

    [HttpGet("denormalized", Name = "GetRbacAssessmentReportDenormalizedDtos")]
    [ProducesResponseType<IEnumerable<RbacAssessmentReportDenormalizedDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<RbacAssessmentReportDenormalizedDto>> GetDenormalized() =>
        await RbacAssessmentReportService.GetRbacAssessmentReportDenormalizedDtos();

    [HttpGet("active-namespaces", Name = "GetRbacAssessmentReportActiveNamespaces")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<string>> GetActiveNamespaces() =>
        await RbacAssessmentReportService.GetActiveNamespaces();
}
