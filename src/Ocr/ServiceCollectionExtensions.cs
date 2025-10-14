using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuroraScienceHub.Framework.Ocr;

/// <summary>
/// Extension methods for setting up OCR services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OCR services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    public static IServiceCollection AddOcr(this IServiceCollection services)
    {
        services.TryAddSingleton<IImageTextReader, ImageTextReader>();

        return services;
    }
}
