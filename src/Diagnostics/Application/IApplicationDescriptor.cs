namespace AuroraScienceHub.Framework.Diagnostics.Application;

/// <summary>
/// Application information descriptor
/// </summary>
public interface IApplicationDescriptor
{
    /// <summary>
    /// Get application information
    /// </summary>
    ApplicationInformation Describe();
}
