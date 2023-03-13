namespace ImageBeautifier.Worker.Models;

internal sealed class BeautifierTask
{
    public Guid Id { get; init; }
    
    public BeautifierTaskState State { get; set; }
    
    public string FileName { get; init; } = string.Empty;
    
    public string OriginalFilePath { get; init; } = string.Empty;
    
    public string? FinishedFilePath { get; set; }
}