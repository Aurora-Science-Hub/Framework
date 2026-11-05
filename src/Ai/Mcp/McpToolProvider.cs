using System.Collections.Concurrent;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace AuroraScienceHub.Framework.Ai.Mcp;

/// <summary>
/// Tools of the MCP servers in <see cref="AiOptions.Mcp"/>
/// </summary>
/// <remarks>
/// <para>
/// A server is connected on the first request, as the Agent Framework samples do: the client and the tool list are
/// kept as long as the application lives, so a new tool of a server appears after the application restarts. A server
/// that cannot be reached is left out and tried again after <see cref="AiOptions.McpRetryInterval"/>, so the assistant
/// answers without its tools meanwhile.
/// </para>
/// <para>
/// Once the tools of a server are loaded, requests get them from the cache without any other work.
/// </para>
/// <para>
/// The attempt to connect to a server is a shared lazy task that concurrent requests await. It is put in place with a
/// compare-and-swap and started only when awaited, so a race never connects twice. It does not depend on the token of
/// the request that started it, so an aborted request does not fail the others.
/// </para>
/// </remarks>
internal sealed class McpToolProvider : IAiToolProvider, IAsyncDisposable
{
    private readonly IOptions<AiOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<McpToolProvider> _logger;
    private readonly ConcurrentDictionary<Uri, ConnectionAttempt> _servers = new();

    public McpToolProvider(
        IOptions<AiOptions> options,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        TimeProvider timeProvider)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
        _loggerFactory = loggerFactory;
        _timeProvider = timeProvider;
        _logger = loggerFactory.CreateLogger<McpToolProvider>();
    }

    public async Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        var tools = new List<AITool>();
        foreach (var (name, server) in options.Mcp)
        {
            if (server.Endpoint is not { } endpoint)
            {
                continue;
            }

            // Loaded before: the tools are given as they are
            var loaded = _servers.GetValueOrDefault(endpoint)?.Connected
                         ?? await GetOrStartAttempt(name, endpoint, options).Server.WaitAsync(cancellationToken);
            if (loaded is not null)
            {
                tools.AddRange(loaded.Tools);
            }
        }

        return tools;
    }

    public async ValueTask DisposeAsync()
    {
        var attempts = _servers.Values.ToList();
        _servers.Clear();

        foreach (var attempt in attempts)
        {
            // An attempt in progress is awaited, so its client is not left open; it ends within ConnectionTimeout
            if (attempt.Started && await attempt.Server is { } connected)
            {
                await connected.Client.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// The attempt to await: the one in place, or a new one when there is none or the last attempt failed
    /// long enough ago
    /// </summary>
    private ConnectionAttempt GetOrStartAttempt(string name, Uri endpoint, AiOptions options)
    {
        while (true)
        {
            var now = _timeProvider.GetUtcNow();
            var current = _servers.GetValueOrDefault(endpoint);
            if (current is not null && !current.IsRetryDue(now, options.McpRetryInterval))
            {
                return current;
            }

            // Not started until awaited: an attempt that loses the race below is dropped without connecting
            var next = new ConnectionAttempt(() => LoadAsync(name, endpoint, options.McpConnectionTimeout), now);
            if (current is null ? _servers.TryAdd(endpoint, next) : _servers.TryUpdate(endpoint, next, current))
            {
                return next;
            }

            // Another request put its attempt in place first: it is the one to await
        }
    }

    /// <summary>
    /// Connects to the server and reads its tools; never throws
    /// </summary>
    /// <returns><c>null</c> if the server cannot be reached</returns>
    private async Task<ServerTools?> LoadAsync(string name, Uri endpoint, TimeSpan connectionTimeout)
    {
        using var timeout = new CancellationTokenSource(connectionTimeout, _timeProvider);
        McpClient? client = null;
        try
        {
            client = await ConnectAsync(name, endpoint, timeout.Token);
            var tools = await client.ListToolsAsync(cancellationToken: timeout.Token);
            _logger.LogInformation(
                "MCP server {McpServer}: {ToolCount} tools ({ToolNames})",
                name,
                tools.Count,
                string.Join(", ", tools.Select(tool => tool.Name)));
            return new ServerTools(client, [.. tools.Select(tool => new McpStructuredResultTool(tool))]);
        }
        catch (Exception exception)
        {
            if (client is not null)
            {
                await client.DisposeAsync();
            }

            _logger.LogWarning(
                exception,
                "MCP server {McpServer} cannot be reached; the assistant answers without its tools for {RetryInterval}",
                name,
                _options.Value.McpRetryInterval);
            return null;
        }
    }

    private async Task<McpClient> ConnectAsync(string name, Uri endpoint, CancellationToken cancellationToken)
    {
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = endpoint,
                Name = name,
                TransportMode = HttpTransportMode.StreamableHttp,
            },
            _httpClientFactory.CreateClient(
                ServiceCollectionExtensions.GetMcpHttpClientName(_options.Value)),
            _loggerFactory,
            ownsHttpClient: true);

        return await McpClient.CreateAsync(transport, clientOptions: null, _loggerFactory, cancellationToken);
    }

    /// <summary>
    /// A client of a server and the tools read through it
    /// </summary>
    private sealed record ServerTools(McpClient Client, IReadOnlyList<AITool> Tools);

    /// <summary>
    /// Attempt to connect to a server and read its tools, begun at <paramref name="startedAt"/>
    /// </summary>
    /// <remarks>
    /// A class, not a record: the compare-and-swap of the dictionary must compare references.
    /// </remarks>
    private sealed class ConnectionAttempt(Func<Task<ServerTools?>> connect, DateTimeOffset startedAt)
    {
        private readonly Lazy<Task<ServerTools?>> _result = new(connect);

        /// <summary>
        /// The tools of the server, <c>null</c> if it cannot be reached; the attempt starts on the first access
        /// </summary>
        public Task<ServerTools?> Server => _result.Value;

        /// <summary>
        /// The server if the attempt has succeeded
        /// </summary>
        public ServerTools? Connected
            => _result.IsValueCreated && _result.Value is { IsCompletedSuccessfully: true, Result: { } connected }
                ? connected
                : null;

        /// <summary>
        /// Whether the attempt has been started
        /// </summary>
        public bool Started => _result.IsValueCreated;

        /// <summary>
        /// Whether the server could not be reached and <paramref name="retryInterval"/> has passed since
        /// </summary>
        public bool IsRetryDue(DateTimeOffset now, TimeSpan retryInterval)
            => Started
               && _result.Value is { IsCompletedSuccessfully: true, Result: null }
               && now - startedAt >= retryInterval;
    }
}
