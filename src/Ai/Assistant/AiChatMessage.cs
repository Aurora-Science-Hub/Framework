namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// One message of a conversation
/// </summary>
/// <param name="Role">Author of the message</param>
/// <param name="Text">Text of the message</param>
public sealed record AiChatMessage(AiRole Role, string Text);
