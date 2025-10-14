using AuroraScienceHub.Framework.Utilities.System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.DateTimes;

/// <summary>
/// Converts <see cref="DateTime"/> to UTC.
/// </summary>
public sealed class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeToUtcConverter()
        : base(input => input.SetUtc(), output => output)
    {
    }
}
