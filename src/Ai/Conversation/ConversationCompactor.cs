using System.Text;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Ai.Conversation;

internal sealed class ConversationCompactor : IConversationCompactor
{
    /// <summary>
    /// Instructions of the summarization request
    /// </summary>
    internal const string Instructions =
        """
        You maintain a running summary of a conversation between a user and an assistant.
        Rewrite the previous summary together with the new messages into one updated summary.
        Keep the user's goals and questions, facts, figures, dates, decisions and open questions; drop greetings and
        repetition. Write plain text without markup, in the language of the conversation, as concisely as possible.
        Answer with the summary only.
        """;

    private const string Operation = "compaction";

    private readonly IChatClient _chatClient;
    private readonly IOptions<AiOptions> _options;
    private readonly AiRequestExecutor _executor;

    public ConversationCompactor(IChatClient chatClient, IOptions<AiOptions> options, AiRequestExecutor executor)
    {
        _chatClient = chatClient;
        _options = options;
        _executor = executor;
    }

    public Task<AiResult> CompactAsync(
        string? previousSummary,
        IReadOnlyList<AiChatMessage> history,
        ChatOptions? callOptions,
        CancellationToken cancellationToken)
    {
        var chatOptions = AiChatOptionsFactory.Create(Instructions, _options.Value, callOptions);
        ChatMessage[] messages = [new(ChatRole.User, BuildPrompt(previousSummary, history))];

        return _executor.ExecuteAsync(
            Operation,
            async token =>
            {
                var response = await _chatClient.GetResponseAsync(messages, chatOptions, token);
                return (response.Text, response.Usage);
            },
            cancellationToken);
    }

    /// <summary>
    /// The previous summary and the new messages as one text
    /// </summary>
    internal static string BuildPrompt(string? previousSummary, IReadOnlyList<AiChatMessage> history)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("Previous summary:");
        prompt.AppendLine(string.IsNullOrWhiteSpace(previousSummary) ? "(none)" : previousSummary);
        prompt.AppendLine();
        prompt.AppendLine("New messages:");

        foreach (var message in history)
        {
            prompt.Append(message.Role == AiRole.User ? "User: " : "Assistant: ").AppendLine(message.Text);
        }

        return prompt.ToString();
    }
}
