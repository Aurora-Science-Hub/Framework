using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuroraScienceHub.Framework.ValueObjects.Accounting;

internal sealed class IdentityUserIdJsonConverter : JsonConverter<IdentityUserId>
{
    public override IdentityUserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("The user subject must be a JSON string.");
        }

        return ReadSubject(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, IdentityUserId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);

    public override IdentityUserId ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        => ReadSubject(ref reader);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, IdentityUserId value, JsonSerializerOptions options)
        => writer.WritePropertyName(value.Value);

    private static IdentityUserId ReadSubject(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        if (String.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("The user subject must not be blank.");
        }

        return new IdentityUserId(value);
    }
}
