using System.IO.Pipelines;
using System.Text.Json;
using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="McpStructuredResultTool"/> against an in-memory MCP server
/// </summary>
public sealed class McpStructuredResultToolTests : IAsyncLifetime
{
    private readonly Pipe _clientToServer = new();
    private readonly Pipe _serverToClient = new();
    private readonly CancellationTokenSource _serverStop = new();
    private McpServer? _server;
    private Task? _serverRun;
    private McpClient? _client;

    public async ValueTask InitializeAsync()
    {
        var options = new McpServerOptions
        {
            ToolCollection =
            [
                McpServerTool.Create(
                    (int count) => new KpSeries([.. Enumerable.Range(1, count).Select(index => new KpRecord(index, 2.7))]),
                    new McpServerToolCreateOptions { Name = "get_kp", UseStructuredContent = true }),
                McpServerTool.Create(
                    string () => throw new McpException("The range is longer than 1 day"),
                    new McpServerToolCreateOptions { Name = "get_too_much" }),
                McpServerTool.Create(() => "Kp is 2.7", new McpServerToolCreateOptions { Name = "get_text" }),
            ],
        };
        _server = McpServer.Create(
            new StreamServerTransport(_clientToServer.Reader.AsStream(), _serverToClient.Writer.AsStream(), "test", NullLoggerFactory.Instance),
            options,
            NullLoggerFactory.Instance,
            serviceProvider: null);
        _serverRun = _server.RunAsync(_serverStop.Token);
        _client = await McpClient.CreateAsync(
            new StreamClientTransport(_clientToServer.Writer.AsStream(), _serverToClient.Reader.AsStream(), NullLoggerFactory.Instance),
            clientOptions: null,
            NullLoggerFactory.Instance,
            TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "Only the structured content reaches the model, not the same data again as escaped text")]
    public async Task Invoke_Structured_ReturnsStructuredContentOnly()
    {
        // Arrange
        var original = await GetToolAsync("get_kp");
        var sut = new McpStructuredResultTool(original);
        var arguments = new AIFunctionArguments { ["count"] = 3 };

        // Act
        var full = Serialize(await original.InvokeAsync(arguments, TestContext.Current.CancellationToken));
        var result = Serialize(await sut.InvokeAsync(arguments, TestContext.Current.CancellationToken));

        // Assert
        sut.Name.ShouldBe("get_kp");
        sut.JsonSchema.GetRawText().ShouldBe(original.JsonSchema.GetRawText());
        using var document = JsonDocument.Parse(result);
        document.RootElement.GetProperty("records").GetArrayLength().ShouldBe(3);
        result.ShouldNotContain("content");
        result.Length.ShouldBeLessThan(full.Length / 2);
    }

    [Fact(DisplayName = "A tool error reaches the model as its text")]
    public async Task Invoke_Error_ReturnsErrorText()
    {
        // Arrange
        var sut = new McpStructuredResultTool(await GetToolAsync("get_too_much"));

        // Act
        var result = await sut.InvokeAsync(new AIFunctionArguments(), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeOfType<string>().ShouldContain("The range is longer than 1 day");
    }

    [Fact(DisplayName = "A result without structured content reaches the model as its text")]
    public async Task Invoke_TextOnly_ReturnsText()
    {
        // Arrange
        var sut = new McpStructuredResultTool(await GetToolAsync("get_text"));

        // Act
        var result = await sut.InvokeAsync(new AIFunctionArguments(), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe("Kp is 2.7");
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }

        await _serverStop.CancelAsync();
        try
        {
            await (_serverRun ?? Task.CompletedTask);
        }
        catch (OperationCanceledException)
        {
            // The server stops with the token
        }

        if (_server is not null)
        {
            await _server.DisposeAsync();
        }

        _serverStop.Dispose();
    }

    private async Task<McpClientTool> GetToolAsync(string name)
    {
        var client = _client.ShouldNotBeNull();
        var tools = await client.ListToolsAsync(cancellationToken: TestContext.Current.CancellationToken);
        return tools.Single(tool => tool.Name == name);
    }

    private static string Serialize(object? value)
        => value is JsonElement element ? element.GetRawText() : JsonSerializer.Serialize(value);

    private sealed record KpSeries(IReadOnlyList<KpRecord> Records);

    private sealed record KpRecord(int Index, double Kp);
}
