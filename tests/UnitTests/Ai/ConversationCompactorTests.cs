using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Extensions.AI;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="ConversationCompactor"/>
/// </summary>
public sealed class ConversationCompactorTests
{
    private static readonly AiChatMessage[] s_turns =
    [
        new(AiRole.User, "I live in Murmansk"),
        new(AiRole.Assistant, "Nice!"),
        new(AiRole.User, "Is Kp 6 today?"),
    ];

    private readonly AiTestHost _host = new();

    [Fact(DisplayName = "The previous summary and the new messages are sent in order with the summary budget")]
    public async Task Compact_SendsSummaryAndTurns()
    {
        // Act
        await _host.Compactor.CompactAsync(
            "Alex asked about auroras.",
            s_turns,
            new ChatOptions { MaxOutputTokens = 200 },
            CancellationToken.None);

        // Assert
        var (messages, options) = _host.ChatClient.Requests.ShouldHaveSingleItem();
        var prompt = messages.ShouldHaveSingleItem().Text;
        prompt.ShouldContain("Alex asked about auroras.");
        prompt.IndexOf("User: I live in Murmansk", StringComparison.Ordinal)
            .ShouldBeLessThan(prompt.IndexOf("Assistant: Nice!", StringComparison.Ordinal));
        prompt.IndexOf("Assistant: Nice!", StringComparison.Ordinal)
            .ShouldBeLessThan(prompt.IndexOf("User: Is Kp 6 today?", StringComparison.Ordinal));
        options.ShouldNotBeNull().Instructions.ShouldBe(ConversationCompactor.Instructions);
        options.MaxOutputTokens.ShouldBe(200);
    }

    [Fact(DisplayName = "Without a previous summary the prompt says so")]
    public async Task Compact_WithoutSummary_MarksNone()
    {
        // Act
        await _host.Compactor.CompactAsync(null, s_turns, callOptions: null, CancellationToken.None);

        // Assert
        _host.ChatClient.Requests.ShouldHaveSingleItem().Messages.ShouldHaveSingleItem().Text.ShouldContain("(none)");
    }

    [Fact(DisplayName = "Without call options the summary budget of the configuration applies")]
    public async Task Compact_NoCallOptions_UsesConfiguredBudget()
    {
        // Act
        await _host.Compactor.CompactAsync(null, s_turns, callOptions: null, CancellationToken.None);

        // Assert
        _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull().MaxOutputTokens.ShouldBe(300);
    }

    [Fact(DisplayName = "The new summary is the text of the answer")]
    public async Task Compact_ReturnsSummary()
    {
        // Arrange
        _host.ChatClient.Respond = _ => Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, "Alex, Murmansk, asks about Kp 6.")));

        // Act
        var result = await _host.Compactor.CompactAsync(null, s_turns, callOptions: null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Text.ShouldBe("Alex, Murmansk, asks about Kp 6.");
    }

    [Fact(DisplayName = "A failure of the provider is reported, not thrown")]
    public async Task Compact_ProviderFails_ReportsOutcome()
    {
        // Arrange
        _host.ChatClient.Respond = _ => throw new HttpRequestException("connection refused");

        // Act
        var result = await _host.Compactor.CompactAsync(null, s_turns, callOptions: null, CancellationToken.None);

        // Assert
        result.Outcome.ShouldBe(AiRequestOutcome.Unavailable);
    }
}
