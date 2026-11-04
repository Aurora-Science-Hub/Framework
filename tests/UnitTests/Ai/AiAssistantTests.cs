using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;
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

    [Fact(DisplayName = "Tools of the request are sent when no provider returns any, and the model is not told they are missing")]
    public async Task Ask_RequestToolsWithoutProviderTools_SendsThem()
    {
        // Arrange
        var request = s_request with
        {
            Options = new ChatOptions { Tools = [AIFunctionFactory.Create(() => "Kp 5", "get_request_tool")] },
        };

        // Act
        await _host.Assistant.AskAsync(request, CancellationToken.None);

        // Assert
        var (messages, options) = _host.ChatClient.Requests.ShouldHaveSingleItem();
        options.ShouldNotBeNull().Tools.ShouldNotBeNull().ShouldHaveSingleItem().Name.ShouldBe("get_request_tool");
        messages.ShouldNotContain(message => message.Text != null && message.Text.Contains(AiAssistant.NoToolsNote));
    }

    [Fact(DisplayName = "Tools of the request and of the providers are all sent, the tools of the request first")]
    public async Task Ask_RequestAndProviderTools_Merged()
    {
        // Arrange
        _host.Tools.Tools.Add(AIFunctionFactory.Create(() => "Kp 5", "get_provider_tool"));
        var request = s_request with
        {
            Options = new ChatOptions { Tools = [AIFunctionFactory.Create(() => "Wind 400", "get_request_tool")] },
        };

        // Act
        await _host.Assistant.AskAsync(request, CancellationToken.None);

        // Assert
        _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull().Tools.ShouldNotBeNull()
            .Select(tool => tool.Name).ShouldBe(["get_request_tool", "get_provider_tool"]);
    }

    [Fact(DisplayName = "A tool of the request named like a provider tool fails the request")]
    public async Task Ask_RequestToolNameConflict_Throws()
    {
        // Arrange
        _host.Tools.Tools.Add(AIFunctionFactory.Create(() => "Kp 5", "get_current_kp_index"));
        var request = s_request with
        {
            Options = new ChatOptions
            {
                Tools = [AIFunctionFactory.Create(() => "Kp 4", "get_current_kp_index")],
            },
        };

        // Act
        var exception = await Should.ThrowAsync<AiToolNameConflictException>(
            () => _host.Assistant.AskAsync(request, CancellationToken.None));

        // Assert
        exception.ToolNames.ShouldBe(["get_current_kp_index"]);
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

    [Fact(DisplayName = "The configured model and generation defaults are sent when the call sets none")]
    public async Task Ask_NoCallOptions_UsesConfiguredDefaults()
    {
        // Arrange
        _host.Options.Model = "default-model";
        _host.Options.Temperature = 0.4f;
        _host.Options.TopP = 0.8f;

        // Act
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        var options = _host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull();
        options.ModelId.ShouldBe("default-model");
        options.Temperature.ShouldBe(0.4f);
        options.TopP.ShouldBe(0.8f);
    }

    [Fact(DisplayName = "Options of a call override the configured defaults and do not affect the next call")]
    public async Task Ask_CallOptions_OverrideDefaultsForOneCall()
    {
        // Arrange
        _host.Options.Model = "default-model";
        _host.Options.MaxOutputTokens = 300;
        _host.Options.Temperature = 0.7f;
        _host.Options.TopP = 0.9f;
        var callOptions = new ChatOptions
        {
            ModelId = "other-model",
            MaxOutputTokens = 42,
            Temperature = 0.1f,
            TopP = 0.5f,
            Reasoning = new ReasoningOptions { Effort = ReasoningEffort.High },
        };

        // Act
        await _host.Assistant.AskAsync(s_request with { Options = callOptions }, CancellationToken.None);
        await _host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        var first = _host.ChatClient.Requests[0].Options.ShouldNotBeNull();
        first.ModelId.ShouldBe("other-model");
        first.MaxOutputTokens.ShouldBe(42);
        first.Temperature.ShouldBe(0.1f);
        first.TopP.ShouldBe(0.5f);
        first.Reasoning.ShouldNotBeNull().Effort.ShouldBe(ReasoningEffort.High);
        callOptions.Instructions.ShouldBeNull();

        var second = _host.ChatClient.Requests[1].Options.ShouldNotBeNull();
        second.ModelId.ShouldBe("default-model");
        second.MaxOutputTokens.ShouldBe(300);
        second.Temperature.ShouldBe(0.7f);
        second.TopP.ShouldBe(0.9f);
        second.Reasoning.ShouldNotBeNull().Effort.ShouldBe(ReasoningEffort.None);
    }

    [Fact(DisplayName = "Tools of every provider are offered to the model")]
    public async Task Ask_SeveralToolProviders_ToolsMerged()
    {
        // Arrange
        var extra = new FakeToolProvider();
        extra.Tools.Add(AIFunctionFactory.Create(() => "second", "second_tool"));
        var host = new AiTestHost(extra);
        host.Tools.Tools.Add(AIFunctionFactory.Create(() => "first", "first_tool"));

        // Act
        await host.Assistant.AskAsync(s_request, CancellationToken.None);

        // Assert
        host.ChatClient.Requests.ShouldHaveSingleItem().Options.ShouldNotBeNull()
            .Tools.ShouldNotBeNull().Select(tool => tool.Name).ShouldBe(["first_tool", "second_tool"]);
    }

    [Fact(DisplayName = "A tool name returned by two providers is reported with the conflicting names")]
    public async Task Ask_DuplicateToolNames_Throws()
    {
        // Arrange
        var extra = new FakeToolProvider();
        extra.Tools.Add(AIFunctionFactory.Create(() => "second", "same_tool"));
        var host = new AiTestHost(extra);
        host.Tools.Tools.Add(AIFunctionFactory.Create(() => "first", "same_tool"));

        // Act
        var exception = await Should.ThrowAsync<AiToolNameConflictException>(
            () => host.Assistant.AskAsync(s_request, CancellationToken.None));

        // Assert
        exception.ToolNames.ShouldBe(["same_tool"]);
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
