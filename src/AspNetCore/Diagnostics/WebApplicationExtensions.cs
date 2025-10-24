using AuroraScienceHub.Framework.AspNetCore.Diagnostics.About;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.AspNetCore.Diagnostics;

/// <summary>
/// <see cref="WebApplication"/> extensions for mapping diagnostic endpoints
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Map diagnostic endpoints
    /// </summary>
    public static void MapDiagnosticEndpoints(
        this WebApplication application,
        IEndpointRouteBuilder? routeBuilder = null)
    {
        routeBuilder ??= application;

        var component = application
            .Services
            .GetRequiredService<GetAboutEndpoint>();

        component.MapEndpoint(routeBuilder);
    }
}
