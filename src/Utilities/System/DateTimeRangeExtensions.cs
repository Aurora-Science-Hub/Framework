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
        => source.Start.EnumerateMinutesTo(source.End);

    /// <summary>
    /// Enumerates all the hours between the start and end date
    /// </summary>
    public static IEnumerable<DateTime> EnumerateHours(this DateTimeRange source)
        => source.Start.EnumerateHoursTo(source.End);
}
