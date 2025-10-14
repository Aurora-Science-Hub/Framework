namespace Ai.Gpt;

/// <summary>
/// Options for the ChatGpt client.
/// </summary>
public sealed class GptOptions
{
    public static readonly string OptionKey = "ChatGpt";

    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan s_defaultMessageExpiration = TimeSpan.FromHours(1);

    /// <summary>
    /// API key
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Default ChatGPT model
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of messages to use for chat completion (default: 10).
    /// </summary>
    public int MessageLimit { get; init; } = 10;

    /// <summary>
    /// Gets or sets the expiration for cached conversation messages (default: 1 hour).
    /// </summary>
    public TimeSpan MessageExpiration { get; init; } = s_defaultMessageExpiration;

    /// <summary>
    /// Gets or sets the timeout for the HTTP requests (default: 1 minute).
    /// </summary>
    public TimeSpan Timeout { get; init; } = s_defaultTimeout;

    /// <summary>
    /// Use a proxy
    /// </summary>
    public bool UseProxy { get; init; } = false;
}
