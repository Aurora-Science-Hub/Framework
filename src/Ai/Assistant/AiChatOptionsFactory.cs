using AuroraScienceHub.Framework.Ai.Options;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// Options of a request to the language model
/// </summary>
internal static class AiChatOptionsFactory
{
    /// <summary>
    /// Options of <paramref name="request"/>, merged with the configured defaults
    /// </summary>
    /// <remarks>
    /// The options of the call are copied first, so neither the caller's instance nor the configured defaults change.
    /// </remarks>
    public static ChatOptions Create(AiRequest request, AiOptions options)
        => Create(request.Instructions, options, request.Options);

    /// <summary>
    /// Options with the instructions, the answer budget, the reasoning effort and the other configured defaults; the
    /// properties set in <paramref name="callOptions"/> win
    /// </summary>
    /// <remarks>
    /// The SDK sends <see cref="ChatOptions.MaxOutputTokens"/> as <c>max_completion_tokens</c>, which DeepSeek ignores;
    /// with <see cref="AiOptions.SendLegacyMaxTokens"/> the legacy <c>max_tokens</c> it honors is set on the raw request
    /// as well.
    /// </remarks>
    public static ChatOptions Create(string instructions, AiOptions options, ChatOptions? callOptions = null)
    {
        var chatOptions = callOptions?.Clone() ?? new ChatOptions();
        chatOptions.Instructions = instructions;
        chatOptions.ModelId ??= options.Model;
        chatOptions.MaxOutputTokens ??= options.MaxOutputTokens;
        chatOptions.Reasoning ??= new ReasoningOptions { Effort = options.ReasoningEffort };
        chatOptions.Temperature ??= options.Temperature;
        chatOptions.TopP ??= options.TopP;

        // Without an answer budget there is no legacy limit to send
        if (options.SendLegacyMaxTokens && chatOptions.MaxOutputTokens is { } maxOutputTokens)
        {
            chatOptions.RawRepresentationFactory ??= _ => WithLegacyMaxTokens(maxOutputTokens);
        }

        return chatOptions;
    }

    private static ChatCompletionOptions WithLegacyMaxTokens(int maxOutputTokens)
    {
        var completionOptions = new ChatCompletionOptions();
        completionOptions.Patch.Set("$.max_tokens"u8, maxOutputTokens);
        return completionOptions;
    }
}
