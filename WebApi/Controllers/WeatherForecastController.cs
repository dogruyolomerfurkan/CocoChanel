using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, Channel<ChannelRequest> channel)
    : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Send(int request)
    {
        await channel.Writer.WriteAsync(new ChannelRequest($"Hello World! {DateTime.UtcNow}", request));

        return Ok();
    }
}