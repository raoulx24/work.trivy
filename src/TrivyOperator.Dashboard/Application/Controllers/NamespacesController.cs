using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Application.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("api/kubernetes-namespaces")]
public class NamespacesController(INamespaceService kubernetesNamespaceService, ILogger<NamespacesController> logger)
    : ControllerBase
{
    [HttpGet(Name = "getAllNamespaces")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<string>> GetAll() => await kubernetesNamespaceService.GetKubernetesNamespaces();
}
