using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.ValueObjects;

/// <summary>
/// Value converter for <see cref="BlobId"/>
/// </summary>
public sealed class BlobIdValueConverter : ValueConverter<BlobId, string>
{
    public BlobIdValueConverter()
        : base(
            input => input.Value,
            output => BlobId.Parse(output),
            new ConverterMappingHints(size: 255))
    {
    }
}
