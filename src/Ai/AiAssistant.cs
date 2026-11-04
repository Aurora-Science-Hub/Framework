using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// The agent runs without a stored session: the conversation is kept by the caller and passed in every request. It may
/// call the tools of <see cref="IAiToolProvider"/> while it answers
/// </summary>
internal sealed class AiAssistant : IAiAssistant
{
    private const string Operation = "answer";

    /// <summary>
    /// Told to the model when no tool can be called, e.g. the MCP server is down: instructions that ask it to look data
    /// up would otherwise make it write a tool call as the answer
    /// </summary>
    internal const string NoToolsNote =
        "No tools are available now: do not try to call any. If the answer needs data you would look up, say that it cannot be looked up right now.";

    private readonly AIAgent _agent;
    private readonly IReadOnlyList<IAiToolProvider> _toolProviders;
    private readonly IOptionsMonitor<AiOptions> _options;
    private readonly AiRequestExecutor _executor;
    private readonly TimeProvider _timeProvider;

    public AiAssistant(
        AIAgent agent,
        IEnumerable<IAiToolProvider> toolProviders,
        IOptionsMonitor<AiOptions> options,
        AiRequestExecutor executor,
        TimeProvider timeProvider)
    {
        _agent = agent;
        _toolProviders = [.. toolProviders];
        _options = options;
        _executor = executor;
        _timeProvider = timeProvider;
    }

    public Task<AiResult> AskAsync(AiRequest request, CancellationToken cancellationToken)
    {
        var options = _options.CurrentValue;
        var chatOptions = AiChatOptionsFactory.Create(request, options);

        return _executor.ExecuteAsync(
            Operation,
            async token =>
            {
                var tools = await GetToolsAsync(token);
                if (tools.Count > 0)
                {
                    chatOptions.Tools = [.. tools];
                }

                var messages = BuildMessages(request, _timeProvider.GetUtcNow(), hasTools: tools.Count > 0);
                var response = await _agent.RunAsync(messages, session: null, new ChatClientAgentRunOptions(chatOptions), token);
                return (FinalText(response.Messages), response.Usage);
            },
            cancellationToken);
    }

    /// <summary>
    /// Tools of all providers, merged; a name returned by two providers is a configuration error
    /// </summary>
    /// <exception cref="AiToolNameConflictException">Two providers returned tools with the same name</exception>
    private async Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken)
    {
        var tools = new List<AITool>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        var conflicts = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var provider in _toolProviders)
        {
            foreach (var tool in await provider.GetToolsAsync(cancellationToken))
            {
                if (names.Add(tool.Name))
                {
                    tools.Add(tool);
                }
                else
                {
                    conflicts.Add(tool.Name);
                }
            }
        }

        if (conflicts.Count > 0)
        {
            throw new AiToolNameConflictException(conflicts);
        }

        return tools;
    }

    /// <summary>
    /// The summary (as a system message), the recent messages, the current time (as a system message, with
    /// <see cref="NoToolsNote"/> when there are no tools) and the question, in this order
    /// </summary>
    /// <remarks>
    /// The time lets the model ask the tools for "now" or "the last day". It comes right before the question, so the
    /// unchanged beginning of the conversation can still be served from the provider's prompt cache.
    /// </remarks>
    internal static List<ChatMessage> BuildMessages(AiRequest request, DateTimeOffset now, bool hasTools)
    {
        var messages = new List<ChatMessage>();

        if (!string.IsNullOrWhiteSpace(request.Summary))
        {
            messages.Add(new ChatMessage(ChatRole.System, $"Summary of the earlier part of the conversation:\n{request.Summary}"));
        }

        messages.AddRange(request.History.Select(turn => new ChatMessage(ToChatRole(turn.Role), turn.Text)));
        var context = $"Current time: {now.UtcDateTime:yyyy-MM-ddTHH:mm}Z";
        messages.Add(new ChatMessage(ChatRole.System, hasTools ? context : $"{context}\n{NoToolsNote}"));
        messages.Add(new ChatMessage(ChatRole.User, request.Question));

        return messages;
    }

    /// <summary>
    /// Text of the last message of the assistant: the answer after all tool rounds
    /// </summary>
    /// <remarks>
    /// The text of the whole response would also contain what the model wrote before calling a tool ("let me look it
    /// up"), which is not part of the answer.
    /// </remarks>
    internal static string? FinalText(IEnumerable<ChatMessage> messages)
        => messages.LastOrDefault(message => message.Role == ChatRole.Assistant)?.Text;

    private static ChatRole ToChatRole(AiRole role)
        => role switch
        {
            AiRole.User => ChatRole.User,
            AiRole.Assistant => ChatRole.Assistant,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Unknown conversation role."),
        };
}
