namespace AuroraScienceHub.Framework.Entities;

/// <summary>
/// Domain object with audit fields
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Date and time of entity creation
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Date and time of entity update
    /// </summary>
    DateTime UpdatedAt { get; }
}
