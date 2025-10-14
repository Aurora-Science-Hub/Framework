using ChatGptNet;
using ChatGptNet.Models;

namespace Ai.Gpt;

internal sealed class GptClient : IGptClient
{
    private readonly IChatGptClient _chatGptClient;

    public GptClient(IChatGptClient chatGptClient)
    {
        _chatGptClient = chatGptClient;
    }

    public async Task<string?> AskAsync(string message, CancellationToken cancellationToken)
    {
        var response = await _chatGptClient.AskAsync(message,
            parameters: new ChatGptParameters
            {
                ResponseFormat = ChatGptResponseFormat.Text,
            },
            cancellationToken: cancellationToken);

        return response.Choices?.FirstOrDefault()?.Message?.Content;
    }
}
