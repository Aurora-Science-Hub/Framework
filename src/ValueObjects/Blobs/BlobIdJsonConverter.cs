using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// JSON converter for <see cref="BlobId"/>
/// </summary>
public sealed class BlobIdJsonConverter : JsonConverter<BlobId>
{
    /// <inheritdoc/>
    public override BlobId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();
        return BlobId.Parse(text);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, BlobId id, JsonSerializerOptions options)
    {
        writer.WriteStringValue(id.Value);
    }

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, BlobId id, JsonSerializerOptions options)
    {
        writer.WritePropertyName(id.Value);
    }

    /// <inheritdoc/>
    public override BlobId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }
}
