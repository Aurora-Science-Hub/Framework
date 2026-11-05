using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// A question to the assistant with the conversation it belongs to
/// </summary>
/// <param name="Instructions">System instructions: the assistant's role and rules</param>
/// <param name="Summary">Summary of the older part of the conversation, if any</param>
/// <param name="History">Recent messages of the conversation, oldest first</param>
/// <param name="Question">The new question</param>
public sealed record AiRequest(string Instructions, string? Summary, IReadOnlyList<AiChatMessage> History, string Question)
{
    /// <summary>
    /// Options of this call, e.g. another model or another answer budget; a property left unset comes from
    /// <see cref="AiOptions"/>
    /// </summary>
    /// <remarks>
    /// The options are copied before they are merged with the configured defaults, so a call never changes them.
    /// <see cref="ChatOptions.Tools"/> of this call are used together with the tools of the
    /// <see cref="Tools.IAiToolProvider"/> services, the tools of the request first; a name used more than once fails
    /// the request with <see cref="Tools.AiToolNameConflictException"/>.
    /// </remarks>
    public ChatOptions? Options { get; init; }
}
