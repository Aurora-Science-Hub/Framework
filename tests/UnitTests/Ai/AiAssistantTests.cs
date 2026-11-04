using AuroraScienceHub.Framework.Ai;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using Shouldly;
using Xunit;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="AiAssistant"/>
/// </summary>
public sealed class AiAssistantTests
{
    private static readonly AiRequest s_request = new(
        "Be helpful",
        "The user lives in Murmansk",
        [new AiChatMessage(AiRole.User, "Hi"), new AiChatMessage(AiRole.Assistant, "Hello!")],
        "What is Kp?");

    private readonly AiTestHost _host = new();

    [Fact(DisplayName = "The summary, the history, the current time and the question are sent in order with the instructions")]
    public async Task Ask_SendsConversationInOrder()
    {
        // Arrange
        _host.Time.SetUtcNow(new DateTimeOffset(2026, 10, 8, 9, 30, 45, TimeSpan.Zero));

        // Act
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        var (messages, options) = _host.ChatClient.Requests.ShouldHaveSingleItem();
        messages.Select(message => message.Role)
            .ShouldBe([ChatRole.System, ChatRole.User, ChatRole.Assistant, ChatRole.System, ChatRole.User]);
        messages[0].Text.ShouldContain("The user lives in Murmansk");
        messages.Skip(1).Select(message => message.Text)
            .ShouldBe(["Hi", "Hello!", $"Current time: 2026-10-08T09:30Z\n{AiAssistant.NoToolsNote}", "What is Kp?"]);
        options.ShouldNotBeNull().Instructions.ShouldBe("Be helpful");
    }

    [Fact(DisplayName = "Without a summary only the current time is sent as a system message")]
    public async Task Ask_WithoutSummary_OnlyTimeSystemMessage()
    {
        // Act
        await _host.Assistant.AskAsync(s_request with { Summary = null }, CancellationToken.None);

        // Assert
        _host.ChatClient.Requests.ShouldHaveSingleItem().Messages
            .Where(message => message.Role == ChatRole.System)
            .ShouldHaveSingleItem().Text.ShouldStartWith("Current time:");
    }

    [Fact(DisplayName = "Without tools the request carries none")]
    public async Task Ask_NoTools_SendsNone()
    {
        // Act
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull().Tools.ShouldBeNull();
    }

    [Fact(DisplayName = "Text the model writes before calling a tool is not part of the answer")]
    public async Task Ask_TextBeforeToolCall_NotInAnswer()
    {
        // Arrange
        _host.Tools.Tools.Add(AIFunctionFactory.Create(() => "Kp 5", "get_current_kp_index"));
        _host.ChatClient.Respond = _ => Task.FromResult(_host.ChatClient.Requests.Count == 1
            ? new ChatResponse(new ChatMessage(
                ChatRole.Assistant,
                [new TextContent("Let me look it up."), new FunctionCallContent("call-1", "get_current_kp_index")]))
            : new ChatResponse(new ChatMessage(ChatRole.Assistant, "Kp is 5 now")));

        // Act
        var result = await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        result.Text.ShouldBe("Kp is 5 now");
    }

    [Fact(DisplayName = "A tool the model calls is invoked and its result goes back to the model for the answer")]
    public async Task Ask_ToolCall_InvokesToolAndAnswers()
    {
        // Arrange
        var calls = 0;
        _host.Tools.Tools.Add(AIFunctionFactory.Create(() => { calls++; return "Kp 5"; }, "get_current_kp_index"));
        _host.ChatClient.Respond = _ => Task.FromResult(_host.ChatClient.Requests.Count == 1
            ? new ChatResponse(new ChatMessage(ChatRole.Assistant, [new FunctionCallContent("call-1", "get_current_kp_index")]))
            : new ChatResponse(new ChatMessage(ChatRole.Assistant, "Kp is 5 now")));

        // Act
        var result = await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        calls.ShouldBe(1);
        result.Text.ShouldBe("Kp is 5 now");
        _host.ChatClient.Requests.Count.ShouldBe(2);
        _host.ChatClient.Requests[0].Messages.ShouldNotContain(message => message.Text.Contains(AiAssistant.NoToolsNote));
        _host.ChatClient.Requests[0].Options.ShouldNotBeNull().Tools.ShouldNotBeNull().ShouldHaveSingleItem().Name.ShouldBe("get_current_kp_index");
        _host.ChatClient.Requests[1].Messages.ShouldContain(message => message.Contents.OfType<FunctionResultContent>().Any());
    }

    [Fact(DisplayName = "Without the legacy option only the standard answer budget is sent")]
    public async Task Ask_WithoutLegacyMaxTokens_NoRawRequest()
    {
        // Act
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        var options = _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull();
        options.MaxOutputTokens.ShouldBe(300);
        options.RawRepresentationFactory.ShouldBeNull();
    }

    [Fact(DisplayName = "The answer budget and the reasoning effort are passed, with a raw request for max_tokens when asked")]
    public async Task Ask_PassesOptions()
    {
        // Arrange
        _host.Options.ReasoningEffort = ReasoningEffort.Low;
        _host.Options.SendLegacyMaxTokens = true;

        // Act
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        var options = _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull();
        options.MaxOutputTokens.ShouldBe(300);
        options.Reasoning.ShouldNotBeNull().Effort.ShouldBe(ReasoningEffort.Low);
        options.RawRepresentationFactory.ShouldNotBeNull()(_host.ChatClient).ShouldBeOfType<ChatCompletionOptions>();
    }

    [Fact(DisplayName = "A successful answer comes with its text and usage")]
    public async Task Ask_Success_ReturnsTextAndUsage()
    {
        // Act
        var result = await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        result.Outcome.ShouldBe(AiRequestOutcome.Success);
        result.Text.ShouldBe("answer");
        result.Usage.ShouldBe(new AiUsage(10, 5));
    }

    [Fact(DisplayName = "An answer without text is reported as empty")]
    public async Task Ask_EmptyAnswer_ReportsEmpty()
    {
        // Arrange
        _host.ChatClient.Respond = _ => Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, " ")));

        // Act
        var result = await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        result.Outcome.ShouldBe(AiRequestOutcome.Empty);
        result.Text.ShouldBeNull();
    }

    [Fact(DisplayName = "A failure of the provider is reported, not thrown")]
    public async Task Ask_ProviderFails_ReportsOutcome()
    {
        // Arrange
        _host.ChatClient.Respond = _ => throw new HttpRequestException("connection refused");

        // Act
        var result = await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        result.Outcome.ShouldBe(AiRequestOutcome.Unavailable);
    }

    [Fact(DisplayName = "No answer within the time budget is reported as a timeout")]
    public async Task Ask_NoAnswerInTime_ReportsTimeout()
    {
        // Arrange
        _host.ChatClient.Respond = async token =>
        {
            await Task.Delay(Timeout.Infinite, token);
            throw new InvalidOperationException("unreachable");
        };

        // Act
        var pending = _host.Assistant.AskAsync(s_request, CancellationToken.None);
        _host.Time.Advance(_host.Options.RequestTimeout);
        var result = await pending;

        // Assert
        result.Outcome.ShouldBe(AiRequestOutcome.Timeout);
    }

    [Fact(DisplayName = "A request aborted by the caller throws a cancellation")]
    public async Task Ask_AbortedByCaller_Throws()
    {
        // Arrange
        using var abort = new CancellationTokenSource();
        _host.ChatClient.Respond = async token =>
        {
            await Task.Delay(Timeout.Infinite, token);
            throw new InvalidOperationException("unreachable");
        };

        // Act
        var pending = _host.Assistant.AskAsync(s_request, abort.Token);
        await abort.CancelAsync();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(() => pending);
    }
}
