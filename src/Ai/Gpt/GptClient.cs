using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai.Gpt;

internal sealed class GptClient : IGptClient
{
    private readonly ChatClient _chatGptClient;

    public GptClient(ChatClient chatGptClient)
    {
        _chatGptClient = chatGptClient;
    }

    public ChatClient Chat => _chatGptClient;

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
}
