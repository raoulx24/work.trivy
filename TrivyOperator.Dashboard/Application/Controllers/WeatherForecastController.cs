using Microsoft.AspNetCore.Mvc;
using TrivyOperator.Dashboard.Domain;
using TrivyOperator.Dashboard.Domain.Services.Abstractions;

namespace TrivyOperator.Dashboard.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    IWeatherForecastDomainService weatherForecastDomainService,
    ILogger<WeatherForecastController> logger) : ControllerBase
{
    [HttpGet(Name = "get")]
    [ProducesResponseType<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IEnumerable<WeatherForecast> Get()
    {
        return weatherForecastDomainService.Get();
    }
}
