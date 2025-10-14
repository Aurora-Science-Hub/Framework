using AuroraScienceHub.Framework.Utilities.System;

namespace AuroraScienceHub.Framework.Utilities;

/// <summary>
/// Date range
/// </summary>
public sealed class DateTimeRange
{
    private const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    /// <summary>
    /// DateTime range
    /// </summary>
    /// <param name="start">Start date-time (UTC, including the boundary)</param>
    /// <param name="end">End date-time (UTC, including the boundary)</param>
    public DateTimeRange(DateTime start, DateTime end)
    {
        if (start > end)
        {
            throw new ArgumentException(
                "Start date-time must be less than or equal to the end date-time",
                nameof(start));
        }

        Start = start;
        End = end;
    }

    /// <summary>Start date-time (UTC, including the boundary)</summary>
    public DateTime Start { get; }

    /// <summary>End date-time (UTC, including the boundary)</summary>
    public DateTime End { get; }

    /// <summary>
    /// Merge two date-time ranges
    /// </summary>
    public DateTimeRange Merge(DateTimeRange? other)
    {
        if (other is null)
        {
            return this;
        }

        return new DateTimeRange(
            DateTimeUtils.Min(Start, other.Start).Required(),
            DateTimeUtils.Max(End, other.End).Required());
    }

    /// <summary>
    /// Deconstructs the date-time range
    /// </summary>
    public void Deconstruct(out DateTime start, out DateTime end)
    {
        start = Start;
        end = End;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Start.ToString(DefaultDateTimeFormat)} - {End.ToString(DefaultDateTimeFormat)}";
}
