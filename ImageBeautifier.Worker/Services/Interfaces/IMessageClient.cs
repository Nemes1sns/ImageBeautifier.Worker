using Amazon.SQS.Model;

namespace ImageBeautifier.Worker.Services.Interfaces;

public interface IMessageClient
{
    Task<IReadOnlyList<Message>> ReceiveMessagesAsync(CancellationToken cancellationToken);
    Task DeleteMessageAsync(Message message, CancellationToken cancellationToken);
}