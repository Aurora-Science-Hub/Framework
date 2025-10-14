namespace AuroraScienceHub.Framework.Entities;

/// <summary>
/// Domain entity with DateTime field
/// </summary>
public interface IHasDateTime
{
    /// <summary>
    /// Date and time of the record (UTC)
    /// </summary>
    DateTime DateTime { get; }
}
