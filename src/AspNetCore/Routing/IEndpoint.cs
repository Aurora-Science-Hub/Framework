using Microsoft.AspNetCore.Routing;

namespace AuroraScienceHub.Framework.AspNetCore.Routing;

/// <summary>
/// Interface for Minimal API endpoint definition
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Configure endpoint
    /// </summary>
    void MapEndpoint(IEndpointRouteBuilder builder);
}
