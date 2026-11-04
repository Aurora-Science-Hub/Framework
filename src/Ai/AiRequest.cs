namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// A question to the assistant with the conversation it belongs to
/// </summary>
/// <param name="Instructions">System instructions: the assistant's role and rules</param>
/// <param name="Summary">Summary of the older part of the conversation, if any</param>
/// <param name="History">Recent messages of the conversation, oldest first</param>
/// <param name="Question">The new question</param>
public sealed record AiRequest(string Instructions, string? Summary, IReadOnlyList<AiChatMessage> History, string Question);
