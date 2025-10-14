using AuroraScienceHub.Framework.Entities;
using AuroraScienceHub.Framework.Entities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuroraScienceHub.Framework.EntityFramework.Interceptors;

/// <summary>
/// Interceptor for saving changes to implement soft deletion
/// </summary>
public sealed class DeletableInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    public DeletableInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var deletedEntities = eventData
            .Context?
            .ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(entry => entry.State == EntityState.Deleted && !entry.Entity.IsDeleted())
            .ToList();

        if (deletedEntities is null || !deletedEntities.Any())
        {
            return ValueTask.FromResult(result);
        }

        var deletedAt = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var deletableEntity in deletedEntities)
        {
            deletableEntity.State = EntityState.Modified;
            deletableEntity.Property(entity => entity.DeletedAt).CurrentValue = deletedAt;
        }

        return ValueTask.FromResult(result);
    }
}
