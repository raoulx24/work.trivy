using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/cluster-compliance-report")]
public class ClusterComplianceReportController(IClusterComplianceReportService clusterComplianceReportService)
    : ControllerBase
{
    [HttpGet(Name = "GetClusterComplianceReportDtos")]
    [ProducesResponseType<IEnumerable<ClusterComplianceReportDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterComplianceReportDto>> Get() =>
        await clusterComplianceReportService.GetClusterComplianceReportDtos();

    [HttpGet("denormalized", Name = "GetClusterComplianceReportDenormalizedDtos")]
    [ProducesResponseType<IEnumerable<ClusterComplianceReportDenormalizedDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterComplianceReportDenormalizedDto>> GetDenormalized() =>
        await clusterComplianceReportService.GetClusterComplianceReportDenormalizedDtos();

    //[HttpGet("summary", Name = "GetClusterComplianceReportSummaryDtos")]
    //[ProducesResponseType<IEnumerable<ClusterRbacAssessmentReportSummaryDto>>(StatusCodes.Status200OK)]
    //[ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    //public async Task<IEnumerable<ClusterRbacAssessmentReportSummaryDto>> GetClusterRbacAssessmentReportSummaryDtos() =>
    //    await clusterComplianceReportService.GetClusterRbacAssessmentReportSummaryDtos();
}
