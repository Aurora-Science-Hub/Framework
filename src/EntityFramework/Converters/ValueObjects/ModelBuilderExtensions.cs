using System.Reflection;
using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// Model builder extensions for value object converters.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies value object converters for all registered value object types.
    /// Supports both nullable and non-nullable value object properties.
    /// </summary>
    public static void UseValueObjectConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(BlobId))
                {
                    continue;
                }

                var isNullable = IsNullableProperty(entityType.ClrType, property.Name);
                property.SetValueConverter(isNullable
                    ? typeof(NullableBlobIdValueConverter)
                    : typeof(BlobIdValueConverter));
            }
        }
    }

    private static bool IsNullableProperty(Type entityType, string propertyName)
    {
        var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo == null)
        {
            return false;
        }

        var nullabilityInfo = new NullabilityInfoContext().Create(propertyInfo);
        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }
}
