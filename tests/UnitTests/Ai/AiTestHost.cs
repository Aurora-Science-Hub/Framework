using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Assistant and compactor wired to a fake chat client and a fake clock
/// </summary>
internal sealed class AiTestHost
{
    public AiTestHost(params IAiToolProvider[] extraToolProviders)
    {
        var options = new StaticOptionsMonitor(Options);
        var executor = new AiRequestExecutor(options, Time, NullLogger<AiRequestExecutor>.Instance);
        Assistant = new AiAssistant(
            ChatClient.AsAIAgent(new Microsoft.Agents.AI.ChatClientAgentOptions()),
            [Tools, .. extraToolProviders],
            options,
            executor,
            Time);
        Compactor = new ConversationCompactor(ChatClient, options, executor);
    }

    public AiOptions Options { get; } = new() { RequestTimeout = TimeSpan.FromMinutes(1), MaxOutputTokens = 300 };

    public FakeChatClient ChatClient { get; } = new();

    public FakeToolProvider Tools { get; } = new();

    public FakeTimeProvider Time { get; } = new();

    public AiAssistant Assistant { get; }

    public ConversationCompactor Compactor { get; }

    private sealed class StaticOptionsMonitor(AiOptions options) : IOptionsMonitor<AiOptions>
    {
        public AiOptions CurrentValue => options;

        public AiOptions Get(string? name) => options;

        public IDisposable? OnChange(Action<AiOptions, string?> listener) => null;
    }
}
