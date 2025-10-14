using System.Reflection;
using AuroraScienceHub.Framework.Utilities.System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using HostOptions = AuroraScienceHub.Framework.Utilities.Configuration.HostOptions;

namespace AuroraScienceHub.Framework.Diagnostics.Application;

internal sealed class ApplicationDescriptor<THostMarker> : IApplicationDescriptor
{
    private static readonly DateTimeOffset s_startedAt = TimeProvider.System.GetUtcNow();

    private readonly IHostEnvironment _hostEnvironment;
    private readonly HostOptions _hostOptions;
    private readonly string _instanceId;

    public ApplicationDescriptor(
        IHostEnvironment hostEnvironment,
        IOptions<HostOptions> options)
    {
        _hostEnvironment = hostEnvironment;
        _hostOptions = options.Value;
        _instanceId = $"{_hostOptions.ResolveHostName()}-{Guid.CreateVersion7()}";
    }

    public ApplicationInformation Describe()
    {
        var assembly = typeof(THostMarker).Assembly;
        var version = GetVersionInfo(assembly);

        return new ApplicationInformation
        (
            _instanceId,
            _hostOptions.ResolveHostName(),
            _hostEnvironment.EnvironmentName,
            _hostOptions.ResolveHostNamespace(),
            s_startedAt,
            version.version,
            version.commitHash
        );
    }

    private static (string version, string? commitHash) GetVersionInfo(Assembly assembly)
    {
        var version = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .Required()
            .InformationalVersion;

        var versionParts = version.Split('+');
        version = versionParts[0];
        var commitHash = versionParts.Length > 1 ? versionParts[1] : null;

        return (version, commitHash);
    }
}
