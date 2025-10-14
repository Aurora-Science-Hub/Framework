using AuroraScienceHub.Framework.Entities.Identifiers;

namespace AuroraScienceHub.Framework.Entities.Storage;

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<TEntity, TEntityId>
    where TEntity : class, IEntity<TEntityId>
    where TEntityId : IIdentifier
{
    Task<TEntity?> FindAsync(TEntityId id, CancellationToken cancellationToken);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);

    Task UpdateRangeAndDetachAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);

    Task AddRangeAndDetachAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);

    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    Task RemoveRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);
}
