namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Extensions for <see cref="DateTimeRange"/>
/// </summary>
public static class DateTimeRangeExtensions
{
    /// <summary>
    /// Enumerates all the minutes between the start and end date
    /// </summary>
    public static IEnumerable<DateTime> EnumerateMinutes(this DateTimeRange source)
    {
        return source.Start.EnumerateMinutesTo(source.End);
    }
}
