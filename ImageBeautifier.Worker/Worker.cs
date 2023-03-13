using ImageBeautifier.Worker.Services.Interfaces;

namespace ImageBeautifier.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageClient _messageClient;

    public Worker(
        ILogger<Worker> logger,
        IMessageClient messageClient)
    {
        _logger = logger;
        _messageClient = messageClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessMessagesAsync(stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var messages = await _messageClient.ReceiveMessagesAsync(cancellationToken);
            foreach (var message in messages)
            {
                await _messageClient.DeleteMessageAsync(message, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}