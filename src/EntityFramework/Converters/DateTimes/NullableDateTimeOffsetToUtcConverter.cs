using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.DateTimes;

/// <summary>
/// Converts nullable <see cref="DateTimeOffset"/> to UTC.
/// </summary>
public sealed class NullableDateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public NullableDateTimeOffsetToUtcConverter()
        : base(input =>
                input.HasValue
                    ? input.Value.ToUniversalTime()
                    : null,
            output => output)
    {
    }
}
