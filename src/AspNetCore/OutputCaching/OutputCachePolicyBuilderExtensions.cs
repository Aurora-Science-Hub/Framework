using Microsoft.AspNetCore.OutputCaching;

namespace AuroraScienceHub.Framework.AspNetCore.OutputCaching;

/// <summary>
/// Output cache policy builder extensions for ASP.NET Core applications.
/// </summary>
public static class OutputCachePolicyBuilderExtensions
{
    /// <summary>
    /// Allows an authenticated request to use the output cache. The default ASP.NET Core policy
    /// rejects every request that carries an <c>Authorization</c> header, so an endpoint that opts
    /// in to caching machine-to-machine calls must re-enable lookup and storage after it.
    /// </summary>
    /// <param name="builder">The output cache policy builder of the endpoint.</param>
    /// <returns>The same builder for chaining.</returns>
    public static OutputCachePolicyBuilder AllowAuthenticated(this OutputCachePolicyBuilder builder)
        => builder.AddPolicy<AuthenticatedOutputCachePolicy>();
}
