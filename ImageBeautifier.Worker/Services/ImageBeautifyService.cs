using System.Diagnostics;
using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS.Model;
using ImageBeautifier.Worker.Models;
using ImageBeautifier.Worker.Services.Interfaces;

namespace ImageBeautifier.Worker.Services;

internal sealed class ImageBeautifyService : IImageBeautifyService
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IImageStorage _imageStorage;
    private readonly ILogger<ImageBeautifyService> _logger;

    public ImageBeautifyService(
        IDynamoDBContext dbContext,
        IImageStorage imageStorage, 
        ILogger<ImageBeautifyService> logger)
    {
        _dbContext = dbContext;
        _imageStorage = imageStorage;
        _logger = logger;
    }
    
    public async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        var task = JsonSerializer.Deserialize<BeautifierTask>(message.Body);
        if (task == null)
        {
            throw new ArgumentException($"The message '{message.MessageId}' isn't correct.");
        }

        if (task.State != BeautifierTaskState.Created)
        {
            _logger.LogWarning($"The message '{message.MessageId}' is being processed already.");
            return;
        }

        await UpdateTaskAsync(task, BeautifierTaskState.InProgress, cancellationToken);

        try
        {
            await ProcessTaskAsync(task, cancellationToken);
        }
        catch (Exception)
        {
            task.FinishedFilePath = null;
            await UpdateTaskAsync(task, BeautifierTaskState.Created, cancellationToken);
            await _dbContext.SaveAsync(task, cancellationToken);

            throw;
        }
    }

    private async Task ProcessTaskAsync(BeautifierTask task, CancellationToken cancellationToken)
    {
        var (image, contentType) = await _imageStorage.GetAsync(task.OriginalFilePath, cancellationToken);
        if (image == null)
        {
            throw new InvalidOperationException($"An image from {task.OriginalFilePath} isn't found.");
        }

        try
        {
            FlipImage(image);

            Debug.Assert(contentType != null, nameof(contentType) + " != null");
            var newPath = await _imageStorage.UploadNewAsync(image, task.FileName, contentType, cancellationToken);

            task.FinishedFilePath = newPath;
            await UpdateTaskAsync(task, BeautifierTaskState.Done, cancellationToken);
        }
        finally
        {
            image.Dispose();
        }
    }

    private void FlipImage(Image image) 
        => image.Mutate(context => context.Flip(FlipMode.Vertical));

    private async Task UpdateTaskAsync(BeautifierTask task, BeautifierTaskState newState, CancellationToken cancellationToken)
    {
        task.State = newState;
        await _dbContext.SaveAsync(task, cancellationToken);
    }
}