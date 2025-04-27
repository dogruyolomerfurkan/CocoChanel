using System.Threading.Channels;

namespace WebApi;

public class Processor(Channel<ChannelRequest> channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            var request = await channel.Reader.ReadAsync(stoppingToken);
            await Task.Delay(3000, stoppingToken);
            Console.WriteLine(request.Message);
        }
    }
}