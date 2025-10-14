namespace AuroraScienceHub.Framework.Composition;

/// <summary>
/// Module options
/// </summary>
public abstract class ApplicationModuleOptionsBase
{
    /// <summary>
    /// Moules configuration section
    /// </summary>
    public const string OptionSectionBase = "Modules";

    /// <summary>
    /// Whether to enable consumers
    /// </summary>
    public bool EnableConsumers { get; set; } = true;

    /// <summary>
    /// Scheduled jobs configuration
    /// </summary>
    public IReadOnlyList<JobOptions> ScheduledJobs { get; set; } = Array.Empty<JobOptions>();

    /// <summary>
    /// Default module options instance
    /// </summary>
    public static ApplicationModuleOptionsBase Default { get; } = new DefaultApplicationModuleOptions();

    /// <summary>
    /// Get job options by name
    /// </summary>
    public JobOptions GetJobOptions(string jobName)
        => ScheduledJobs
               .FirstOrDefault(x => x.Name == jobName)
           ?? JobOptions.Default;
}

/// <summary>
/// Default application module options
/// </summary>
public sealed class DefaultApplicationModuleOptions : ApplicationModuleOptionsBase;

/// <summary>
/// Job options
/// </summary>
public sealed class JobOptions
{
    /// <summary>
    /// Job name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Execution period
    /// </summary>
    public TimeSpan? Period { get; set; }

    /// <summary>
    /// Whether to enable the job
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Default job options instance
    /// </summary>
    public static JobOptions Default { get; } = new();
}
