using Amazon.S3;
using Amazon.SQS;
using ImageBeautifier.Worker;
using ImageBeautifier.Worker.Infrastructure.Configuration;
using ImageBeautifier.Worker.Services;
using ImageBeautifier.Worker.Services.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<AWSEnvironmentOptions>(context.Configuration.GetSection("AWSEnvironment"));
        
        services.AddSingleton<IImageBeautifyService, ImageBeautifyService>();
        services.AddSingleton<IImageStorage, ImageStorage>();
        services.AddSingleton<IMessageClient, MessageClient>();
        
        var awsOptions = context.Configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
        services.AddAWSService<IAmazonSQS>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();