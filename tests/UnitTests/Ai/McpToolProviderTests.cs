using AuroraScienceHub.Framework.Ai;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="McpToolProvider"/>
/// </summary>
public sealed class McpToolProviderTests : IAsyncDisposable
{
    private readonly AiOptions _options = new();
    private readonly FakeTimeProvider _time = new();
    private readonly CountingHandler _handler = new();
    private readonly McpToolProvider _sut;

    public McpToolProviderTests()
    {
        _sut = new McpToolProvider(new StaticOptionsMonitor(_options), new HandlerClientFactory(_handler), NullLoggerFactory.Instance, _time);
    }

    [Fact(DisplayName = "Without configured servers no tools are offered and nothing is connected")]
    public async Task GetTools_NoServers_Empty()
    {
        // Arrange
        _options.Mcp["core"] = new AiMcpServerOptions();

        // Act
        var tools = await _sut.GetToolsAsync(CancellationToken.None);

        // Assert
        tools.ShouldBeEmpty();
        _handler.Requests.ShouldBe(0);
    }

    [Fact(DisplayName = "An unreachable server is left out and tried again only after the configured retry interval")]
    public async Task GetTools_Unreachable_LeftOutUntilRetry()
    {
        // Arrange
        _options.Mcp["core"] = new AiMcpServerOptions { Endpoint = new Uri("http://core.test/mcp") };
        _options.McpRetryInterval = TimeSpan.FromMinutes(2);

        // Act
        var first = await _sut.GetToolsAsync(CancellationToken.None);
        var requestsAfterFirst = _handler.Requests;
        var second = await _sut.GetToolsAsync(CancellationToken.None);
        var requestsAfterSecond = _handler.Requests;
        _time.Advance(_options.McpRetryInterval - TimeSpan.FromSeconds(1));
        await _sut.GetToolsAsync(CancellationToken.None);
        var requestsBeforeInterval = _handler.Requests;
        _time.Advance(TimeSpan.FromSeconds(1));
        await _sut.GetToolsAsync(CancellationToken.None);

        // Assert
        first.ShouldBeEmpty();
        second.ShouldBeEmpty();
        requestsAfterFirst.ShouldBeGreaterThan(0);
        requestsAfterSecond.ShouldBe(requestsAfterFirst);
        requestsBeforeInterval.ShouldBe(requestsAfterSecond);
        _handler.Requests.ShouldBeGreaterThan(requestsBeforeInterval);
    }

    [Fact(DisplayName = "Concurrent first requests share one connection attempt")]
    public async Task GetTools_Concurrent_ConnectOnce()
    {
        // Arrange
        _options.Mcp["core"] = new AiMcpServerOptions { Endpoint = new Uri("http://core.test/mcp") };
        await using (var single = new McpToolProvider(
            new StaticOptionsMonitor(_options),
            new HandlerClientFactory(_handler),
            NullLoggerFactory.Instance,
            _time))
        {
            await single.GetToolsAsync(CancellationToken.None);
        }

        var requestsOfOneAttempt = _handler.Requests;

        // Act
        await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => _sut.GetToolsAsync(CancellationToken.None)));

        // Assert
        requestsOfOneAttempt.ShouldBeGreaterThan(0);
        (_handler.Requests - requestsOfOneAttempt).ShouldBe(requestsOfOneAttempt);
    }

    [Fact(DisplayName = "A cancelled request is not reported as an unreachable server")]
    public async Task GetTools_Cancelled_Throws()
    {
        // Arrange
        _options.Mcp["core"] = new AiMcpServerOptions { Endpoint = new Uri("http://core.test/mcp") };
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() => _sut.GetToolsAsync(cancellation.Token));
    }

    public async ValueTask DisposeAsync()
    {
        await _sut.DisposeAsync();
        _handler.Dispose();
    }

    private sealed class StaticOptionsMonitor(AiOptions options) : IOptionsMonitor<AiOptions>
    {
        public AiOptions CurrentValue => options;

        public AiOptions Get(string? name) => options;

        public IDisposable? OnChange(Action<AiOptions, string?> listener) => null;
    }

    private sealed class HandlerClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    /// <summary>
    /// Handler of a server that refuses every connection
    /// </summary>
    private sealed class CountingHandler : HttpMessageHandler
    {
        private int _requests;

        public int Requests => _requests;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _requests);
            throw new HttpRequestException("Connection refused");
        }
    }
}
