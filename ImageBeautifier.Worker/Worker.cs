using Amazon.SQS.Model;
using ImageBeautifier.Worker.Services;
using ImageBeautifier.Worker.Services.Interfaces;

namespace ImageBeautifier.Worker;

public class Worker : BackgroundService
{
    private readonly IImageBeautifyService _imageBeautifyService;
    private readonly ILogger<Worker> _logger;
    private readonly IMessageClient _messageClient;

    public Worker(
        IImageBeautifyService imageBeautifyService,
        ILogger<Worker> logger,
        IMessageClient messageClient)
    {
        _imageBeautifyService = imageBeautifyService;
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
        IReadOnlyList<Message> messages;
        try
        {
            messages = await _messageClient.ReceiveMessagesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return;
        }
        
        foreach (var message in messages)
        {
            try
            {
                await _imageBeautifyService.ProcessMessage(message, cancellationToken);
                // await _messageClient.DeleteMessageAsync(message, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}