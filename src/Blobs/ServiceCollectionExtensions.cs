using Amazon.S3;
using AuroraScienceHub.Framework.Blobs.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Extensions for registering S3 blob storage services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds S3 blob storage services to the service collection.
    /// </summary>
    public static IServiceCollection AddS3Blobs(this IServiceCollection services)
    {
        services.AddOptions<S3Options>().BindConfiguration(S3Options.SectionKey);

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<S3Options>>().Value;
            var config = new AmazonS3Config
            {
                ServiceURL = options.RequiredServerUrl,
                ForcePathStyle = true,
                UseHttp = !options.UseHttps
            };
            return new AmazonS3Client(options.RequiredAccessKey, options.RequiredSecretKey, config);
        });

        services.AddTransient<IBlobClient, S3BlobClient>();

        return services;
    }
}
