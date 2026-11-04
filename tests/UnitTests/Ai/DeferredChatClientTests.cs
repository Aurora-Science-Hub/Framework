using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="DeferredChatClient"/>
/// </summary>
public sealed class DeferredChatClientTests
{
    private readonly MutableOptionsMonitor _options = new(new AiOptions());
    private readonly FakeChatClient _inner = new();
    private readonly List<AiOptions> _createdWith = [];
    private readonly DeferredChatClient _sut;

    public DeferredChatClientTests()
    {
        _sut = new DeferredChatClient(_options, options =>
        {
            _createdWith.Add(options);
            return _inner;
        });
    }

    [Fact(DisplayName = "Asking for a service does not create the provider client")]
    public void GetService_NoCall_ClientNotCreated()
    {
        // Act
        var service = _sut.GetService(typeof(IChatClient));

        // Assert
        service.ShouldBeSameAs(_sut);
        _createdWith.ShouldBeEmpty();
    }

    [Fact(DisplayName = "The first call creates the provider client from the options of that moment")]
    public async Task GetResponseAsync_FirstCall_CreatesClientFromCurrentOptions()
    {
        // Arrange
        var options = new AiOptions { Model = "after-the-reload" };
        _options.CurrentValue = options;

        // Act
        await _sut.GetResponseAsync([new ChatMessage(ChatRole.User, "Hi")], cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _createdWith.ShouldHaveSingleItem().ShouldBeSameAs(options);
        _inner.Requests.ShouldHaveSingleItem();
    }

    [Fact(DisplayName = "The created provider client is kept, so the settings it captured need a restart")]
    public async Task GetResponseAsync_SettingsChangedAfterCreation_ClientKept()
    {
        // Arrange
        await _sut.GetResponseAsync([new ChatMessage(ChatRole.User, "Hi")], cancellationToken: TestContext.Current.CancellationToken);
        _options.CurrentValue = new AiOptions { Model = "after-the-change" };

        // Act
        await _sut.GetResponseAsync([new ChatMessage(ChatRole.User, "Hi again")], cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _createdWith.ShouldHaveSingleItem();
        _inner.Requests.Count.ShouldBe(2);
    }

    private sealed class MutableOptionsMonitor(AiOptions options) : IOptionsMonitor<AiOptions>
    {
        public AiOptions CurrentValue { get; set; } = options;

        public AiOptions Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<AiOptions, string?> listener) => null;
    }
}
