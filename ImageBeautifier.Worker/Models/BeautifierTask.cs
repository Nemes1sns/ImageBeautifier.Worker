using Amazon.DynamoDBv2.DataModel;

namespace ImageBeautifier.Worker.Models;

[DynamoDBTable("BeautifierTask")]
internal sealed class BeautifierTask
{
    [DynamoDBHashKey]
    public Guid Id { get; init; }
    
    [DynamoDBProperty]
    public BeautifierTaskState State { get; set; }
    
    [DynamoDBProperty]
    public string FileName { get; init; } = string.Empty;
    
    [DynamoDBProperty]
    public string OriginalFilePath { get; init; } = string.Empty;
    
    [DynamoDBProperty]
    public string? FinishedFilePath { get; set; }
}