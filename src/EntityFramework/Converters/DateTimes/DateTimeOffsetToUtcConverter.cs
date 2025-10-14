using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.DateTimes;

/// <summary>
/// Converts <see cref="DateTimeOffset"/> to UTC.
/// </summary>
public sealed class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetToUtcConverter()
        : base(input => input.ToUniversalTime(), output => output)
    {
    }
}
