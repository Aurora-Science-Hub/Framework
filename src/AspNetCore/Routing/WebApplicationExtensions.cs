using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.AspNetCore.Routing;

/// <summary>
/// <see cref="WebApplication"/> extensions for mapping endpoints
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Add endpoints from all registered definitions
    /// </summary>
    public static void MapEndpoints(
        this WebApplication application,
        IEndpointRouteBuilder? routeBuilder = null)
    {
        var components = application
            .Services
            .GetServices<IEndpoint>()
            .ToArray();

        routeBuilder ??= application;

        foreach (var component in components)
        {
            component.MapEndpoint(routeBuilder);
        }
    }
}
