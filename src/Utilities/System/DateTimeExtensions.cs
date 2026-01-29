namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Extensions for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
    private static readonly TimeSpan s_minute = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan s_hour = TimeSpan.FromHours(1);

    /// <summary>
    /// Creates new <see cref="DateTime"/> instance from input
    /// with <see cref="DateTimeKind"/> equal to <see cref="DateTimeKind.Utc"/>
    /// </summary>
    public static DateTime SetUtc(this DateTime value)
        => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };

    /// <summary>
    /// Enumerates all minutes between the start and end date
    /// </summary>
    public static IEnumerable<DateTime> EnumerateMinutesTo(this DateTime start, DateTime end)
        => start > end
            ? throw new ArgumentException("Start date must be before end date.")
            : EnumerateToInternal(start, end, s_minute);

    /// <summary>
    /// Enumerates all hours between the start and end date
    /// </summary>
    public static IEnumerable<DateTime> EnumerateHoursTo(this DateTime start, DateTime end)
        => start > end
            ? throw new ArgumentException("Start date must be before end date.")
            : EnumerateToInternal(start, end, s_hour);

    /// <summary>
    /// Enumerates all minutes between the start and end date with the specified interval
    /// </summary>
    public static IEnumerable<DateTime> EnumerateTo(this DateTime start, DateTime end, TimeSpan interval)
        => start > end
            ? throw new ArgumentException("Start date must be before end date.")
            : EnumerateToInternal(start, end, interval);

    /// <summary>
    /// Converts UTC to local DateTime with specified hours offset
    /// </summary>
    /// <param name="xData">DateTime in UTC</param>
    /// <param name="offset">UTC offset</param>
    public static DateTime[] ApplyOffset(this DateTime[] xData, TimeSpan offset)
        => xData.Select(x => x.AddHours(offset.Hours)).ToArray();

    private static IEnumerable<DateTime> EnumerateToInternal(this DateTime start, DateTime end, TimeSpan interval)
    {
        for (var dateTime = start; dateTime <= end; dateTime = dateTime.Add(interval))
        {
            yield return dateTime;
        }
    }
}
