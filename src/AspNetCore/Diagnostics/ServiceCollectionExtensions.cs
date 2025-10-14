using AuroraScienceHub.Framework.AspNetCore.Diagnostics.About;
using AuroraScienceHub.Framework.Diagnostics.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuroraScienceHub.Framework.AspNetCore.Diagnostics;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add diagnostic services
    /// </summary>
    public static void AddDiagnosticEndpoints<THostMarker>(this IServiceCollection services)
    {
        services.AddApplicationDescriptor<THostMarker>();
        services.TryAddTransient<GetAboutEndpoint>();
    }
}
