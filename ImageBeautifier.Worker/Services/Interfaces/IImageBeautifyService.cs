using Amazon.SQS.Model;

namespace ImageBeautifier.Worker.Services.Interfaces;

public interface IImageBeautifyService
{
    Task ProcessMessage(Message message, CancellationToken cancellationToken);
}