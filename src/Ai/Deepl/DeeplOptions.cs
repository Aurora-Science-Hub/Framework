namespace AuroraScienceHub.Framework.Ai.Deepl;

/// <summary>
/// Options for the DeepL client.
/// </summary>
public sealed class DeeplOptions
{
    public static readonly string OptionKey = "DeepL";

    /// <summary>
    /// API key
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Use a proxy
    /// </summary>
    public bool UseProxy { get; init; } = false;
}
