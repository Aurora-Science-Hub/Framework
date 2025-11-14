namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// <see cref="IEnumerable{T}"/> extensions
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Filters out null values from the source enumerable
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(source);

        foreach (var item in source)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Filters out null values from the source enumerable
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(source);

        foreach (var item in source)
        {
            if (item.HasValue)
            {
                yield return item.Value;
            }
        }
    }
}
