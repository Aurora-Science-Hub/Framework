using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Options of a request to the language model
/// </summary>
internal static class AiChatOptionsFactory
{
    /// <summary>
    /// Options with the instructions, the answer budget and the reasoning effort
    /// </summary>
    /// <remarks>
    /// The SDK sends <see cref="ChatOptions.MaxOutputTokens"/> as <c>max_completion_tokens</c>, which DeepSeek ignores;
    /// with <see cref="AiOptions.SendLegacyMaxTokens"/> the legacy <c>max_tokens</c> it honors is set on the raw request
    /// as well.
    /// </remarks>
    public static ChatOptions Create(string instructions, int maxOutputTokens, AiOptions options)
        => new()
        {
            Instructions = instructions,
            MaxOutputTokens = maxOutputTokens,
            Reasoning = new ReasoningOptions { Effort = options.ReasoningEffort },
            RawRepresentationFactory = options.SendLegacyMaxTokens ? _ => WithLegacyMaxTokens(maxOutputTokens) : null,
        };

    private static ChatCompletionOptions WithLegacyMaxTokens(int maxOutputTokens)
    {
        var completionOptions = new ChatCompletionOptions();
        completionOptions.Patch.Set("$.max_tokens"u8, maxOutputTokens);
        return completionOptions;
    }
}
