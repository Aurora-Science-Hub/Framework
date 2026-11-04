using AuroraScienceHub.Framework.Ai.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Chat client of the provider, created by the first model call
/// </summary>
/// <remarks>
/// The services are registered even while the AI features are disabled, so the assistant and the compactor must be
/// resolvable without a key and a model, and a configuration reload before the first call must be taken into account.
/// The SDK validates the key in its constructor, so the client is built by the first call instead: a caller makes it
/// only when <see cref="AiOptions.Enabled"/> is set.
/// <para>
/// The client captures <see cref="AiOptions.Provider"/>, <see cref="AiOptions.Endpoint"/>, <see cref="AiOptions.ApiKey"/>,
/// <see cref="AiOptions.UseProxy"/>, <see cref="AiOptions.RequestTimeout"/>, <see cref="AiOptions.MaxRetries"/> and
/// <see cref="AiOptions.MaxToolCallRounds"/> when it is created; a change of these settings needs a restart. The other
/// settings are read for every request.
/// </para>
/// </remarks>
internal sealed class DeferredChatClient : IChatClient
{
    private readonly IOptionsMonitor<AiOptions> _options;
    private readonly Func<AiOptions, IChatClient> _create;
    private readonly Lock _gate = new();
    private IChatClient? _inner;

    public DeferredChatClient(IOptionsMonitor<AiOptions> options, Func<AiOptions, IChatClient> create)
    {
        _options = options;
        _create = create;
    }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => GetClient().GetResponseAsync(messages, options, cancellationToken);

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => GetClient().GetStreamingResponseAsync(messages, options, cancellationToken);

    /// <summary>
    /// A service of the client, once it is created; asking for a service does not create it, because the agent asks
    /// what the client supports while the AI features may still be disabled
    /// </summary>
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceKey is null && serviceType == typeof(IChatClient))
        {
            return this;
        }

        lock (_gate)
        {
            return _inner?.GetService(serviceType, serviceKey);
        }
    }

    public void Dispose()
    {
        IChatClient? inner;
        lock (_gate)
        {
            inner = _inner;
            _inner = null;
        }

        inner?.Dispose();
    }

    private IChatClient GetClient()
    {
        lock (_gate)
        {
            return _inner ??= _create(_options.CurrentValue);
        }
    }
}
