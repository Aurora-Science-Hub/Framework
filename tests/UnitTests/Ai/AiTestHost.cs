using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Assistant and compactor wired to a fake chat client and a fake clock
/// </summary>
internal sealed class AiTestHost
{
    public AiTestHost(params IAiToolProvider[] extraToolProviders)
    {
        var options = Microsoft.Extensions.Options.Options.Create(Options);
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
}
