using Amazon.SQS;
using Amazon.SQS.Model;
using ImageBeautifier.Worker.Infrastructure.Configuration;
using ImageBeautifier.Worker.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ImageBeautifier.Worker.Services;

internal sealed class MessageClient : IMessageClient
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly AWSEnvironmentOptions _awsEnvironmentOptions;

    public MessageClient(
        IAmazonSQS amazonSqs,
        IOptions<AWSEnvironmentOptions> awsEnvironmentOptions)
    {
        _amazonSqs = amazonSqs;
        _awsEnvironmentOptions = awsEnvironmentOptions.Value;
    }
    
    public async Task<IReadOnlyList<Message>> ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _awsEnvironmentOptions.QueueUrl,
            WaitTimeSeconds = _awsEnvironmentOptions.QueueWaitTimeSeconds,
            MaxNumberOfMessages = _awsEnvironmentOptions.QueueMaxNumberOfMessages
        };
        var response = await _amazonSqs.ReceiveMessageAsync(request, cancellationToken);
        return response.Messages;
    }

    public async Task DeleteMessageAsync(Message message, CancellationToken cancellationToken) 
        => await _amazonSqs.DeleteMessageAsync(_awsEnvironmentOptions.QueueUrl, message.ReceiptHandle, cancellationToken);
}