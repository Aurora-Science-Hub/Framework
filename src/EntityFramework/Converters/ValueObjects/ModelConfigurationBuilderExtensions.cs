using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// <see cref="ModelConfigurationBuilder"/> extensions for value object converters.
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Applies value object converters for all registered value object types.
    /// Supports both nullable and non-nullable value object properties.
    /// </summary>
    public static ModelConfigurationBuilder UseValueObjectsConvensions(this ModelConfigurationBuilder modelBuilder)
    {
        UseBlobIdConvention(modelBuilder);

        return modelBuilder;
    }

    public static ModelConfigurationBuilder UseBlobIdConvention(this ModelConfigurationBuilder builder)
    {
        builder.Properties<BlobId>()
               .HaveMaxLength(BlobId.MaxLength)
               .HaveConversion<BlobIdValueConverter>();

        builder.Properties<BlobId?>()
               .HaveMaxLength(BlobId.MaxLength)
               .HaveConversion<NullableBlobIdValueConverter>();

        return builder;
    }
}
