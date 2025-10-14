using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.Unicode.UnicodeRanges;

namespace AuroraScienceHub.Framework.Json;

/// <summary>
/// Default json serializer options
/// </summary>
public static class DefaultJsonSerializerOptions
{
    private static readonly JavaScriptEncoder s_cyrillicEncoder = JavaScriptEncoder.Create(
        BasicLatin,
        Cyrillic);

    /// <summary>
    /// Create new instance of <see cref="JsonSerializerOptions"/> with default settings
    /// </summary>
    public static JsonSerializerOptions Create() => Configure(new JsonSerializerOptions());

    /// <summary>
    /// Configure <see cref="JsonSerializerOptions"/> with default settings
    /// </summary>
    public static JsonSerializerOptions Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Encoder = s_cyrillicEncoder;
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
