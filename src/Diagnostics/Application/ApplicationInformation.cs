namespace AuroraScienceHub.Framework.Diagnostics.Application;

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
public sealed record class ApplicationInformation(
    string InstanceId,
    string ApplicationName,
    string EnvironmentName,
    string HostNamespace,
    DateTimeOffset StartedAt,
    string Version,
    string? CommitHash);
