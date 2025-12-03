using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// Model builder extensions for value object converters.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly Dictionary<Type, Type> s_converters = new()
    {
        { typeof(BlobId), typeof(BlobIdValueConverter) },
    };

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
                var propertyType = property.ClrType;
                var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (s_converters.TryGetValue(underlyingType, out var converterType))
                {
                    property.SetValueConverter(converterType);
                }
            }
        }
    }

}
