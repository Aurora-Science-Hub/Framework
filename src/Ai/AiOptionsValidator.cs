using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Validates <see cref="AiOptions"/> while AI is enabled; the messages never contain the API key
/// </summary>
internal sealed class AiOptionsValidator : IValidateOptions<AiOptions>
{
    /// <summary>
    /// Upper bound of <see cref="AiOptions.MaxRetries"/>
    /// </summary>
    internal const int MaxRetriesLimit = 5;

    public ValidateOptionsResult Validate(string? name, AiOptions options)
    {
        // Nothing is sent to the provider while AI is disabled
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Skip;
        }

        var failures = new List<string>();

        // Plain HTTP only for a local model: the API key must never leave the host unencrypted
        if (options.Endpoint is not { IsAbsoluteUri: true } endpoint
            || (endpoint.Scheme != Uri.UriSchemeHttps && !(endpoint.Scheme == Uri.UriSchemeHttp && endpoint.IsLoopback)))
        {
            failures.Add($"{Key(nameof(AiOptions.Endpoint))} must be an absolute HTTPS address (plain HTTP only for a loopback address)");
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            failures.Add($"{Key(nameof(AiOptions.ApiKey))} is required");
        }

        if (string.IsNullOrWhiteSpace(options.Model))
        {
            failures.Add($"{Key(nameof(AiOptions.Model))} is required");
        }

        if (options.RequestTimeout <= TimeSpan.Zero)
        {
            failures.Add($"{Key(nameof(AiOptions.RequestTimeout))} must be positive");
        }

        // No output limit is a valid value: the provider default applies
        if (options.MaxOutputTokens is <= 0)
        {
            failures.Add($"{Key(nameof(AiOptions.MaxOutputTokens))} must be positive when set");
        }

        if (options.Temperature is < 0 or > 2)
        {
            failures.Add($"{Key(nameof(AiOptions.Temperature))} must be between 0 and 2");
        }

        if (options.TopP is < 0 or > 1)
        {
            failures.Add($"{Key(nameof(AiOptions.TopP))} must be between 0 and 1");
        }

        if (options.MaxRetries is < 0 or > MaxRetriesLimit)
        {
            failures.Add($"{Key(nameof(AiOptions.MaxRetries))} must be between 0 and {MaxRetriesLimit}");
        }

        // No upper bound: RequestTimeout limits the whole loop anyway
        if (options.MaxToolCallRounds < 1)
        {
            failures.Add($"{Key(nameof(AiOptions.MaxToolCallRounds))} must be positive");
        }

        if (options.McpConnectionTimeout <= TimeSpan.Zero)
        {
            failures.Add($"{Key(nameof(AiOptions.McpConnectionTimeout))} must be positive");
        }

        if (options.McpRetryInterval <= TimeSpan.Zero)
        {
            failures.Add($"{Key(nameof(AiOptions.McpRetryInterval))} must be positive");
        }

        foreach (var (serverName, server) in options.Mcp)
        {
            // No secret is sent to an MCP server, so plain HTTP is fine (e.g. a server in the same private network)
            if (server.Endpoint is { } mcpEndpoint
                && (!mcpEndpoint.IsAbsoluteUri || (mcpEndpoint.Scheme != Uri.UriSchemeHttps && mcpEndpoint.Scheme != Uri.UriSchemeHttp)))
            {
                failures.Add($"{Key($"{nameof(AiOptions.Mcp)}:{serverName}:{nameof(AiMcpServerOptions.Endpoint)}")} must be an absolute HTTP(S) address");
            }
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    private static string Key(string property) => $"{AiOptions.OptionKey}:{property}";
}
