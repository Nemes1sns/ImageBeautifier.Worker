namespace ImageBeautifier.Worker.Infrastructure.Configuration;

public sealed class AWSEnvironmentOptions
{
    public string QueueUrl { get; init; } = string.Empty;
    public int QueueWaitTimeSeconds { get; init; }
    public int QueueMaxNumberOfMessages { get; init; }
   
}