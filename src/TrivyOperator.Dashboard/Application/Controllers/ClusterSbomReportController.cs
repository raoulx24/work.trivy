using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Models;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/cluster-sbom-reports")]
public class ClusterSbomReportController(IClusterSbomReportService clusterSbomReportService) : ControllerBase
{
    [HttpGet(Name = "GetClusterSbomReportDtos")]
    [ProducesResponseType<IEnumerable<ClusterSbomReportDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ClusterSbomReportDto>> Get() =>
        await clusterSbomReportService.GetClusterSbomReportDtos();
}
