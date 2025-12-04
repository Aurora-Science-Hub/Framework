using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// Model builder extensions for value object converters.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly Dictionary<Type, (Type NonNullable, Type Nullable)> s_converters = new()
    {
        { typeof(BlobId), (typeof(BlobIdValueConverter), typeof(NullableBlobIdValueConverter)) },
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

                // For reference types, check if the property is nullable via NullabilityInfoContext
                if (s_converters.TryGetValue(propertyType, out var converterTypes))
                {
                    var isNullable = IsNullableReferenceType(entityType.ClrType, property.Name);
                    var converterType = isNullable ? converterTypes.Nullable : converterTypes.NonNullable;
                    property.SetValueConverter(converterType);
                }
            }
        }
    }

    private static bool IsNullableReferenceType(Type entityType, string propertyName)
    {
        var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo == null)
        {
            return false;
        }

        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(propertyInfo);

        return nullabilityInfo.WriteState == NullabilityState.Nullable
            || nullabilityInfo.ReadState == NullabilityState.Nullable;
    }
}
