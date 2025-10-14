using AuroraScienceHub.Framework.AspNetCore.Routing;
using AuroraScienceHub.Framework.Diagnostics.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AuroraScienceHub.Framework.AspNetCore.Diagnostics.About;

internal sealed class GetAboutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                ApiResource.Diagnostics.About,
                (
                    HttpContext _,
                    [FromServices] IApplicationDescriptor applicationDescriptor) =>
                {
                    var applicationDescription = applicationDescriptor.Describe();
                    return Results.Ok(new ApplicationInformationResponse(
                        applicationDescription.InstanceId,
                        applicationDescription.ApplicationName,
                        applicationDescription.EnvironmentName,
                        applicationDescription.HostNamespace,
                        applicationDescription.StartedAt,
                        applicationDescription.Version,
                        applicationDescription.CommitHash));
                })
            .WithName("About")
            .WithDescription("Get the application info")
            .WithTags(nameof(ApiResource.Diagnostics));
    }
}

/// <summary>
/// Application information
/// </summary>
/// <param name="InstanceId">Application instance identifier</param>
/// <param name="ApplicationName">Application name</param>
/// <param name="EnvironmentName">Environment name</param>
/// <param name="HostNamespace">Host namespace</param>
/// <param name="StartedAt">Application start time</param>
/// <param name="Version">Application version</param>
/// <param name="CommitHash">Commit hash. Can be <see langword="null"/> if the application is not running from a git repository</param>
public sealed record class ApplicationInformationResponse(
    string InstanceId,
    string ApplicationName,
    string EnvironmentName,
    string HostNamespace,
    DateTimeOffset StartedAt,
    string Version,
    string? CommitHash);
