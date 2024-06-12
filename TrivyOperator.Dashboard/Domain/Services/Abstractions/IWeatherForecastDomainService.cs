namespace TrivyOperator.Dashboard.Domain.Services.Abstractions;

public interface IWeatherForecastDomainService
{
    IEnumerable<WeatherForecast> Get();
}
