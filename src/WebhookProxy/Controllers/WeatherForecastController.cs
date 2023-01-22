using Heimdall.CommonServices.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Heimdall.WebhookProxy.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IStorageAccess storageAccess;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IStorageAccess storageAccess)
    {
        this._logger = logger;
        this.storageAccess = storageAccess;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var devices = this.storageAccess.GetDevicesAsync().GetAwaiter().GetResult();

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
