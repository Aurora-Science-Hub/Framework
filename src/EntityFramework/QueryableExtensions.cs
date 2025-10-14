using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework;

/// <summary>
/// Extension methods for <see cref="IQueryable{TSource}"/>
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Asynchronously creates a read-only list from the query.
    /// </summary>
    public static async Task<IReadOnlyList<TSource>> ToReadOnlyListAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken)
    {
        return await source.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}
