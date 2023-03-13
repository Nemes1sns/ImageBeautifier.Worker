namespace ImageBeautifier.Worker.Services.Interfaces;

internal interface IImageStorage
{
    Task<(Image? image, string? contentType)> GetAsync(string path, CancellationToken cancellationToken);
    Task<string> UploadNewAsync(Image image, string fileName, string contentType, CancellationToken cancellationToken);
}