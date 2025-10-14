namespace AuroraScienceHub.Framework.Entities.Specifications;

/// <summary>
/// Specification extensions for <see cref="IQueryable{TEntity}"/>
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Returns only not soft deleted entities
    /// </summary>
    public static IQueryable<TEntity> NotDeleted<TEntity>(this IQueryable<TEntity> source)
        where TEntity : ISoftDeletable
    {
        return source.Where(x => x.DeletedAt == null);
    }

    /// <summary>
    /// Returns only soft deleted entities
    /// </summary>
    public static IQueryable<TEntity> Deleted<TEntity>(this IQueryable<TEntity> source)
        where TEntity : ISoftDeletable
    {
        return source.Where(x => x.DeletedAt != null);
    }
}
