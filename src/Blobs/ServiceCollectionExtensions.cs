using AuroraScienceHub.Framework.Blobs.S3;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Регистрация сервисов
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить сервисы блобов Minio
    /// </summary>
    public static IServiceCollection AddMinioBlobs(this IServiceCollection services)
    {
        services.AddOptions<S3Options>().BindConfiguration(S3Options.SectionKey);


        services.AddTransient<IBlobClient, S3BlobClient>();

        return services;
    }
}
