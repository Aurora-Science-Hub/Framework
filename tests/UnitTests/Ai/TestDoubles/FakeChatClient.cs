using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;

/// <summary>
/// Chat client that records the requests and answers with a configured response
/// </summary>
internal sealed class FakeChatClient : IChatClient
{
    public List<(List<ChatMessage> Messages, ChatOptions? Options)> Requests { get; } = [];

    /// <summary>
    /// Produces the response of a request; by default a fixed answer
    /// </summary>
    public Func<CancellationToken, Task<ChatResponse>> Respond { get; set; } = _ => Task.FromResult(
        new ChatResponse(new ChatMessage(ChatRole.Assistant, "answer"))
        {
            Usage = new UsageDetails { InputTokenCount = 10, OutputTokenCount = 5 },
        });

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Requests.Add((messages.ToList(), options));
        return Respond(cancellationToken);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public object? GetService(Type serviceType, object? serviceKey = null)
        => serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;

    public void Dispose()
    {
    }
}
