using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Options of the language model provider
/// </summary>
public sealed class AiOptions
{
    /// <summary>
    /// Default configuration section
    /// </summary>
    public const string OptionKey = "Ai";

    /// <summary>
    /// Whether the AI features are enabled; while disabled, no request is sent to the provider and the other settings
    /// are not validated
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Kind of the provider API
    /// </summary>
    public AiProvider Provider { get; set; } = AiProvider.OpenAICompatible;

    /// <summary>
    /// Base address of the provider API, e.g. https://api.deepseek.com
    /// </summary>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// API key of the provider; a secret, never logged
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Model to use, e.g. deepseek-flash
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Time budget of one request, all attempts included; also the limit of each attempt
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// How many times a request that failed for a transient reason (rate limit, server error) is retried
    /// </summary>
    /// <remarks>
    /// An attempt may take the whole <see cref="RequestTimeout"/>, so an attempt that timed out is never retried: the
    /// budget is spent. Retries help with failures that come quickly.
    /// </remarks>
    public int MaxRetries { get; set; } = 1;

    /// <summary>
    /// Maximum number of tokens in an answer; <c>null</c> means the limit is not sent and the provider default applies
    /// </summary>
    /// <remarks>
    /// The value is an upper bound, not a target size of the answer. It can be overridden for a single call with
    /// <see cref="ChatOptions.MaxOutputTokens"/>.
    /// </remarks>
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    /// Whether the answer budget is also sent as the legacy <c>max_tokens</c>, for providers that ignore
    /// <c>max_completion_tokens</c> (DeepSeek); OpenAI rejects <c>max_tokens</c> for its reasoning models
    /// </summary>
    public bool SendLegacyMaxTokens { get; set; }

    /// <summary>
    /// Reasoning effort of models that support it; <see cref="Microsoft.Extensions.AI.ReasoningEffort.None"/>
    /// disables reasoning (faster and cheaper answers)
    /// </summary>
    public ReasoningEffort ReasoningEffort { get; set; } = ReasoningEffort.None;

    /// <summary>
    /// Temperature of the answer for models that support it; <c>null</c> means the provider default applies
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// Top-P of the answer for models that support it; <c>null</c> means the provider default applies
    /// </summary>
    public float? TopP { get; set; }

    /// <summary>
    /// Whether requests go through the shared proxy (the <c>Proxy</c> section)
    /// </summary>
    public bool UseProxy { get; set; }

    /// <summary>
    /// MCP servers whose tools the assistant may call, by name
    /// </summary>
    public Dictionary<string, AiMcpServerOptions> Mcp { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Maximum number of model calls for one answer when the model calls tools: each round of tool calls takes one more
    /// </summary>
    /// <remarks>
    /// All the calls share <see cref="RequestTimeout"/>.
    /// </remarks>
    public int MaxToolCallRounds { get; set; } = 5;

    /// <summary>
    /// Time to connect to an MCP server and read its tools; a server that does not answer must not take the time budget
    /// of the request
    /// </summary>
    public TimeSpan McpConnectionTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Pause before an MCP server that could not be reached is tried again
    /// </summary>
    public TimeSpan McpRetryInterval { get; set; } = TimeSpan.FromMinutes(1);
}
