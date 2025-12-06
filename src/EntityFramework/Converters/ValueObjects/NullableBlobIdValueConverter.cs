using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// Value converter for nullable <see cref="BlobId"/>
/// </summary>
public sealed class NullableBlobIdValueConverter : ValueConverter<BlobId?, string?>
{
    public NullableBlobIdValueConverter()
        : base(
            input => input == null ? null : input.Value,
            output => string.IsNullOrEmpty(output) ? null : BlobId.Parse(output),
            new ConverterMappingHints(size: BlobId.MaxLength))
    {
    }
}

