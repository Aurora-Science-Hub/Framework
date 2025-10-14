using AuroraScienceHub.Framework.Utilities.System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.DateTimes;

/// <summary>
/// Converts nullable <see cref="DateTime"/> to UTC.
/// </summary>
public sealed class NullableDateTimeToUtcConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeToUtcConverter()
        : base(input =>
                input.HasValue
                    ? input.Value.SetUtc()
                    : null,
            output => output)
    {
    }
}
