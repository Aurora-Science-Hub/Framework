using System.Text.Json;

namespace AuroraScienceHub.Framework.Json;

/// <summary>
/// Default json serializer
/// </summary>
public static class DefaultJsonSerializer
{
    private static readonly JsonSerializerOptions s_options = DefaultJsonSerializerOptions.Create();

    /// <summary>
    /// Serialize value to json string
    /// </summary>
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, s_options);

    /// <summary>
    /// Deserialize json string to value
    /// </summary>
    public static T? Deserialize<T>(string state) => JsonSerializer.Deserialize<T>(state, s_options);
}
