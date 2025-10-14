using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.DateTimes;

/// <summary>
/// Model builder extensions for date time converters.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Uses <see cref="DateTimeOffsetToUtcConverter"/> and <see cref="NullableDateTimeOffsetToUtcConverter"/> for all <see cref="DateTimeOffset"/> properties
    /// to convert them to UTC.
    /// </summary>
    public static void UseDateTimeOffsetToUtcConversion(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(new DateTimeOffsetToUtcConverter());
                }
                else if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(new NullableDateTimeOffsetToUtcConverter());
                }
            }
        }
    }

    /// <summary>
    /// Uses <see cref="DateTimeToUtcConverter"/> and <see cref="NullableDateTimeToUtcConverter"/> for all <see cref="DateTime"/> properties
    /// to convert them to UTC.
    /// </summary>
    public static void UseDateTimeToUtcConversion(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new DateTimeToUtcConverter());
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new NullableDateTimeToUtcConverter());
                }
            }
        }
    }
}
