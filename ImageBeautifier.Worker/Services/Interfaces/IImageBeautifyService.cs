using Amazon.SQS.Model;

namespace ImageBeautifier.Worker.Services.Interfaces;

public interface IImageBeautifyService
{
    Task ProcessMessageAsync(Message message, CancellationToken cancellationToken);
}