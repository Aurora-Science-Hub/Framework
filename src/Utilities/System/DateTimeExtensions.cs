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
    /// Truncates a timestamp to the UTC minute (seconds and below cleared).
    /// Accepts <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Unspecified"/>
    /// (Unspecified is treated as UTC). Throws for <see cref="DateTimeKind.Local"/> —
    /// machine time zone must not be used for UTC minute normalization
    /// </summary>
    /// <param name="value">UTC or Unspecified timestamp</param>
    /// <exception cref="ArgumentException">When <paramref name="value"/> has <see cref="DateTimeKind.Local"/></exception>
    public static DateTime TruncateToUtcMinute(this DateTime value)
    {
        if (value.Kind == DateTimeKind.Local)
        {
            throw new ArgumentException(
                "Only UTC or Unspecified DateTime is accepted; Local is rejected because the machine time zone is not trusted.",
                nameof(value));
        }

        var utc = value.SetUtc();
        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, second: 0, DateTimeKind.Utc);
    }

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
