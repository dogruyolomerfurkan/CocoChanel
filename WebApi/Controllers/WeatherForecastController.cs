using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, Channel<ChannelRequest> channel)
    : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Send()
    {
        await channel.Writer.WriteAsync(new ChannelRequest($"Hello World! {DateTime.UtcNow}"));

        return Ok();
    }
}