using AuroraScienceHub.Framework.Entities;
using AuroraScienceHub.Framework.Entities.Identifiers;
using AuroraScienceHub.Framework.Entities.Storage;
using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework.Storage;

public abstract class DefaultRepository<TContext, TEntity, TEntityId> :
    IRepository<TEntity, TEntityId>
    where TContext : DbContext
    where TEntity : class, IEntity<TEntityId>
    where TEntityId : class, IIdentifier
{
    protected readonly TContext Context;

    protected readonly DbSet<TEntity> DbSet;

    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>>? _sort;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>>? _filter;

    public DefaultRepository(
        TContext context,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? sort = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter = null)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
        _sort = sort;
        _filter = filter;
    }

    public async Task UpdateRangeAndDetachAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return;
        }

        DbSet.UpdateRange(entities);
        await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return;
        }

        await DbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddRangeAndDetachAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return;
        }

        DbSet.AddRange(entities);
        await Context.SaveChangesAsync(cancellationToken);

        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }
    }

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity is ISoftDeletable softDeletableEntity)
        {
            var entry = Context.Entry(softDeletableEntity);

            entry
                .Property(e => e.DeletedAt)
                .CurrentValue = TimeProvider.System.GetUtcNow().UtcDateTime;
            entry.State = EntityState.Modified;
            return Context.SaveChangesAsync(cancellationToken);
        }

        DbSet.Remove(entity);
        return Context.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return Task.CompletedTask;
        }

        if (typeof(TEntity).IsAssignableTo(typeof(ISoftDeletable)))
        {
            var softDeletableEntities = entities.Cast<ISoftDeletable>().ToList();
            var deletedAt = TimeProvider.System.GetUtcNow().UtcDateTime;
            foreach (var entity in softDeletableEntities)
            {
                var entry = Context.Entry(entity);

                entry.Property(e => e.DeletedAt).CurrentValue = deletedAt;
                entry.State = EntityState.Modified;
            }

            return Context.SaveChangesAsync(cancellationToken);
        }

        DbSet.RemoveRange(entities);
        return Context.SaveChangesAsync(cancellationToken);
    }

    public Task<TEntity?> FindAsync(TEntityId id, CancellationToken cancellationToken)
    {
        return DbSet.FindAsync([id], cancellationToken).AsTask();
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Update(entity);
        return Context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return Task.CompletedTask;
        }

        DbSet.UpdateRange(entities);
        return Context.SaveChangesAsync(cancellationToken);
    }

    protected IQueryable<TEntity> Read()
    {
        var query = DbSet.AsQueryable();

        query = _filter is null ? query : _filter(query);
        query = _sort is null ? query : _sort(query);

        return query;
    }
}
