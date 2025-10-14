namespace Ai.Gpt;

public interface IGptClient
{
    Task<string?> AskAsync(string message, CancellationToken cancellationToken);
}
