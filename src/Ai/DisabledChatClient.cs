using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Chat client of a service whose AI features are disabled
/// </summary>
/// <remarks>
/// The assistant and the compactor are registered even while the AI features are disabled, so the service graph must be
/// resolvable without a key and a model, which the SDK rejects in its constructor. This client lets it be resolved; a
/// call reaches it only when the caller does not check <see cref="Options.AiOptions.Enabled"/> first, so the call
/// throws instead of silently doing nothing.
/// </remarks>
internal sealed class DisabledChatClient : IChatClient
{
    public static DisabledChatClient Instance { get; } = new();

    private DisabledChatClient()
    {
    }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("The AI features are disabled.");

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("The AI features are disabled.");

    public object? GetService(Type serviceType, object? serviceKey = null)
        => serviceKey is null && serviceType == typeof(IChatClient) ? this : null;

    public void Dispose()
    {
    }
}
