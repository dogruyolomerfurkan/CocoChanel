using System.Threading.Channels;
using Polly;
using Polly.Retry;

namespace WebApi;

public class Processor(Channel<ChannelRequest> channel) : BackgroundService
{
    private readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>().Or<NotImplementedException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} due to: {exception.Message}");
                }
            );
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            ChannelRequest? request = null;

            try
            {
                request = await channel.Reader.ReadAsync(stoppingToken);
                //ExecuteAndCapture da catche düşmez. Result olarak handle edebilirsin
                await _retryPolicy.ExecuteAsync(async ct =>
                {
                    await Task.Delay(3000, ct);

                    Console.WriteLine(request.Message + " and Request number " + request.request);

                    throw new Exception("blabla");

                }, stoppingToken);
            }
            catch (Exception ex)
            {
                // This catch only fires if all retries failed (or if an unexpected exception)
                Console.WriteLine($"Failed to process '{request?.Message}': {ex.Message}");

                // Decide whether to swallow or rethrow.
                // If you want to stop the service entirely on a persistent failure:
                // throw;
                // Otherwise, continue processing next messages:
            }
        }
    }
}