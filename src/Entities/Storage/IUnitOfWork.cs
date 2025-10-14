using System.Data;

namespace AuroraScienceHub.Framework.Entities.Storage;

/// <summary>
/// Represents a unit of work interface for managing database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new unit of work.
    /// </summary>
    Task BeginAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current unit of work.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current unit of work if an error occurs or if the transaction needs to be aborted.
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
