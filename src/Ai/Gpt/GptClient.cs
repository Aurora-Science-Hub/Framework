using AuroraScienceHub.Framework.Json;
using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai.Gpt;

internal sealed class GptClient : IGptClient
{
    private readonly ChatClient _chatGptClient;

    public GptClient(ChatClient chatGptClient)
    {
        _chatGptClient = chatGptClient;
    }

    public async Task<string?> AskAsync(string message, CancellationToken cancellationToken)
    {
        var response = await _chatGptClient.CompleteChatAsync(
            messages: [message],
            cancellationToken: cancellationToken);

        if (response.Value.Content != null && response.Value.Content.Count > 0)
        {
            return response.Value.Content[0].Text;
        }
        return null;
    }

    public async Task<TResult> RequestAndDeserializeAsync<TResult>(
        string systemMessage,
        string prompt,
        CancellationToken cancellationToken)
        where TResult : class
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemMessage),
            new SystemChatMessage("Your response must be in valid JSON format."),
            prompt,
        };

        var gptResponse = await _chatGptClient.CompleteChatAsync(
                messages: messages,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var responseJson = string.Empty;
        if (gptResponse.Value.Content is { Count: > 0 })
        {
            responseJson = gptResponse.Value.Content[0].Text?.Trim('`');
        }

        if (string.IsNullOrWhiteSpace(responseJson))
        {
            throw new InvalidOperationException("GPT response is empty");
        }

        var summaryResults = DefaultJsonSerializer.Deserialize<TResult>(responseJson)
                             ?? throw new InvalidOperationException("Failed to deserialize GPT response");
        return summaryResults;
    }
}
