namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Tokens spent on a request, as reported by the provider
/// </summary>
/// <param name="InputTokens">Tokens of the request</param>
/// <param name="OutputTokens">Tokens of the answer</param>
public sealed record AiUsage(long? InputTokens, long? OutputTokens);
