using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuroraScienceHub.Framework.Entities.Identifiers;

/// <summary>
/// JSON converter for <see cref="IIdentifier{T}"/>
/// </summary>
/// <typeparam name="T">Type of identifier</typeparam>
public sealed class IdentifierJsonConverter<T> : JsonConverter<T>
    where T : IIdentifier<T>
{
    /// <inheritdoc/>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();
        return T.Parse(text);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T id, JsonSerializerOptions options)
    {
        writer.WriteStringValue(id.Value);
    }

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T id, JsonSerializerOptions options)
    {
        writer.WritePropertyName(id.Value);
    }

    /// <inheritdoc/>
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }
}
