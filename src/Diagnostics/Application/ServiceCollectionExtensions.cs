using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuroraScienceHub.Framework.Diagnostics.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add application descriptor
    /// </summary>
    public static void AddApplicationDescriptor<THostMarker>(this IServiceCollection services)
    {
        services.TryAddSingleton<IApplicationDescriptor, ApplicationDescriptor<THostMarker>>();
    }
}
