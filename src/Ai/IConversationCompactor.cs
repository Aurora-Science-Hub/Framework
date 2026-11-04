using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Folds older messages of a conversation into a running summary
/// </summary>
public interface IConversationCompactor
{
    /// <summary>
    /// Writes a new summary from the previous one and the messages that follow it
    /// </summary>
    /// <remarks>
    /// Failures are reported like those of <see cref="IAiAssistant.AskAsync"/>; the text of a successful result is the
    /// new summary.
    /// </remarks>
    /// <param name="previousSummary">Summary of the part of the conversation before <paramref name="history"/>, if any</param>
    /// <param name="history">Messages to fold into the summary, oldest first</param>
    /// <param name="callOptions">
    /// Options of this call, e.g. <see cref="ChatOptions.MaxOutputTokens"/> for the summary budget; a property left unset
    /// comes from <see cref="AiOptions"/>
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<AiResult> CompactAsync(
        string? previousSummary,
        IReadOnlyList<AiChatMessage> history,
        ChatOptions? callOptions,
        CancellationToken cancellationToken);
}
