using System.Diagnostics.CodeAnalysis;

namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// Result of a request to the language model
/// </summary>
/// <param name="Outcome">Outcome of the request</param>
/// <param name="Text">Text of the answer; set only on <see cref="AiRequestOutcome.Success"/></param>
/// <param name="Usage">Tokens spent, if the provider reported them</param>
public sealed record AiResult(AiRequestOutcome Outcome, string? Text, AiUsage? Usage)
{
    /// <summary>
    /// Whether the model answered with text
    /// </summary>
    [MemberNotNullWhen(true, nameof(Text))]
    public bool IsSuccess => Outcome == AiRequestOutcome.Success && Text is not null;

    /// <summary>
    /// A failed request
    /// </summary>
    public static AiResult Failed(AiRequestOutcome outcome, AiUsage? usage = null) => new(outcome, null, usage);
}
