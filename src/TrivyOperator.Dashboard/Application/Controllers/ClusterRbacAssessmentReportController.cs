using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;
using TrivyOperator.Dashboard.Utils;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/cluster-rbac-assessment-reports")]
public class ClusterRbacAssessmentReportController(
    IClusterRbacAssessmentReportService clusterRbacAssessmentReportService): ControllerBase
{
    [HttpGet(Name = "GetClusterRbacAssessmentReportDtos")]
    [ProducesResponseType<IEnumerable<ClusterRbacAssessmentReportDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterRbacAssessmentReportDto>> Get()
    {
        return await clusterRbacAssessmentReportService.GetClusterRbacAssessmentReportDtos();
    }

    [HttpGet("denormalized", Name = "GetClusterRbacAssessmentReportDenormalizedDto")]
    [ProducesResponseType<IEnumerable<ClusterRbacAssessmentReportDenormalizedDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterRbacAssessmentReportDenormalizedDto>> GetDenormalized()
    {
        return await clusterRbacAssessmentReportService.GetClusterRbacAssessmentReportDenormalizedDtos();
    }

    [HttpGet("summary", Name = "GetClusterRbacAssessmentReportSummaryDtos")]
    [ProducesResponseType<IEnumerable<ClusterRbacAssessmentReportSummaryDto>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterRbacAssessmentReportSummaryDto>> GetClusterRbacAssessmentReportSummaryDtos()
    {
        return await clusterRbacAssessmentReportService.GetClusterRbacAssessmentReportSummaryDtos();
    }
}
