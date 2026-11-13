using System.Security.Claims;
using AuroraScienceHub.Framework.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.AspNetCore.OutputCaching;

/// <summary>
/// Tests for <see cref="AuthenticatedOutputCachePolicy"/>.
/// </summary>
public sealed class AuthenticatedOutputCachePolicyTests
{
    [Theory(DisplayName = "The request phase composes the default policy with the authenticated opt-in")]
    [InlineData("GET", true, true)]
    [InlineData("HEAD", true, true)]
    [InlineData("GET", false, true)]
    [InlineData("POST", true, false)]
    public async Task EndpointPolicy_RequestPhase(string method, bool authenticated, bool expected)
    {
        // Act
        var result = await RunEndpointPolicyAsync(method, authenticated);

        // Assert
        result.Lookup.ShouldBe(expected);
        result.Storage.ShouldBe(expected);
    }

    [Theory(DisplayName = "The response phase keeps storage only for a 200 response without cookies")]
    [InlineData(StatusCodes.Status200OK, false, true)]
    [InlineData(StatusCodes.Status200OK, true, false)]
    [InlineData(StatusCodes.Status500InternalServerError, false, false)]
    public async Task EndpointPolicy_ResponsePhase(int statusCode, bool setCookie, bool expectedStorage)
    {
        // Act
        var result = await RunEndpointPolicyAsync(
            HttpMethods.Get,
            authenticated: true,
            statusCode,
            setCookie);

        // Assert
        result.Storage.ShouldBe(expectedStorage);
    }

    private static async Task<(bool Lookup, bool Storage)> RunEndpointPolicyAsync(
        string method,
        bool authenticated,
        int statusCode = StatusCodes.Status200OK,
        bool setCookie = false)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddOutputCache();
        await using var app = builder.Build();
        app.MapGet("/cached", () => "ok")
            .CacheOutput(policy => policy.AllowAuthenticated().Expire(TimeSpan.FromSeconds(1)));

        var endpoint = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(route => route.RoutePattern.RawText == "/cached");

        var policy = endpoint.Metadata.GetMetadata<IOutputCachePolicy>();
        policy.ShouldNotBeNull();

        var context = CreateContext(method, authenticated);
        context.HttpContext.RequestServices = app.Services;
        context.HttpContext.Response.StatusCode = statusCode;
        if (setCookie)
        {
            context.HttpContext.Response.Headers.SetCookie = "session=test";
        }

        await policy!.CacheRequestAsync(context, TestContext.Current.CancellationToken);
        var lookup = context.AllowCacheLookup;
        await policy.ServeResponseAsync(context, TestContext.Current.CancellationToken);

        return (lookup, context.AllowCacheStorage);
    }

    private static OutputCacheContext CreateContext(string method, bool authenticated)
    {
        var identity = authenticated
            ? new ClaimsIdentity([new Claim(ClaimTypes.Name, "test")], "test")
            : new ClaimsIdentity();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
        };
        httpContext.Request.Method = method;

        return new OutputCacheContext
        {
            HttpContext = httpContext,
        };
    }
}
