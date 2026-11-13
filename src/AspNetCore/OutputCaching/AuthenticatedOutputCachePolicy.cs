using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace AuroraScienceHub.Framework.AspNetCore.OutputCaching;

/// <summary>
/// Re-enables output caching for an authenticated request after the default ASP.NET Core policy
/// turned it off. Use it only for endpoints whose response does not depend on the caller.
/// </summary>
public sealed class AuthenticatedOutputCachePolicy : IOutputCachePolicy
{
    /// <inheritdoc />
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        if (IsCacheableRequest(context))
        {
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = true;
            context.AllowCacheStorage = true;
            context.AllowLocking = true;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    /// <summary>
    /// The default policy of ASP.NET Core 11.0.0-rc.2 disables storage for every authenticated
    /// response in this phase, so the opt-in has to be repeated after it. The check is equivalent
    /// to the default one and does nothing on 11.0.0 GA, where the default policy already keeps an
    /// explicitly allowed authenticated response.
    /// </summary>
    /// <inheritdoc />
    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        if (IsCacheableRequest(context)
            && StringValues.IsNullOrEmpty(response.Headers.SetCookie)
            && response.StatusCode == StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = true;
        }

        return ValueTask.CompletedTask;
    }

    private static bool IsCacheableRequest(OutputCacheContext context)
    {
        var request = context.HttpContext.Request;

        return context.HttpContext.User.Identity?.IsAuthenticated == true
            && (HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method));
    }
}
