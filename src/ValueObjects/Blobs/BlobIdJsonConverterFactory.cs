using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// JSON converter factory for <see cref="BlobId"/>
/// </summary>
public sealed class BlobIdJsonConverterFactory : JsonConverterFactory
{
    private static readonly BlobIdJsonConverter s_converter = new();

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(BlobId);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return s_converter;
    }
}

