using Amazon.S3;
using Amazon.S3.Model;
using ImageBeautifier.Worker.Infrastructure.Configuration;
using ImageBeautifier.Worker.Services.Interfaces;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Advanced;

namespace ImageBeautifier.Worker.Services;

internal sealed class ImageStorage : IImageStorage
{
    private readonly IAmazonS3 _amazonS3;
    private readonly AWSEnvironmentOptions _awsEnvironmentOptions;
    private const string NewFilePrefix = "new";

    public ImageStorage(IAmazonS3 amazonS3, IOptions<AWSEnvironmentOptions> awsEnvironmentOptions)
    {
        _amazonS3 = amazonS3;
        _awsEnvironmentOptions = awsEnvironmentOptions.Value;
    }

    
    public async Task<(Image? image, string? contentType)> GetAsync(string path, CancellationToken cancellationToken)
    {
        var response = await _amazonS3.GetObjectAsync(_awsEnvironmentOptions.BucketName, path, cancellationToken);
        var contentType = response.Metadata["Content-Type"];
        var image = await Image.LoadAsync(response.ResponseStream, cancellationToken);
        return (image, contentType);
    }

    public async Task<string> UploadNewAsync(Image image, string fileName, string contentType, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await image.SaveAsync(stream, image.DetectEncoder(fileName), cancellationToken);
        var request = new PutObjectRequest
        {
            BucketName = _awsEnvironmentOptions.BucketName,
            Key = $"{NewFilePrefix}/{fileName}",
            InputStream = stream
        };
        request.Metadata.Add("Content-Type", contentType);
        await _amazonS3.PutObjectAsync(request, cancellationToken);
        return request.Key;
    }
}