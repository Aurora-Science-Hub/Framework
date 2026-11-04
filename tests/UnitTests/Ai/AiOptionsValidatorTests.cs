using AuroraScienceHub.Framework.Ai;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="AiOptionsValidator"/>
/// </summary>
public sealed class AiOptionsValidatorTests
{
    private readonly AiOptionsValidator _sut = new();

    [Fact(DisplayName = "Valid options pass")]
    public void Validate_Valid_Succeeds()
    {
        // Arrange
        var options = new AiOptions { Enabled = true, Endpoint = new Uri("https://ai.example.org"), ApiKey = "key", Model = "model" };

        // Act & Assert
        _sut.Validate(name: null, options).Succeeded.ShouldBeTrue();
    }

    [Fact(DisplayName = "Every invalid setting is reported at once, without the API key")]
    public void Validate_Invalid_ReportsAll()
    {
        // Arrange
        var options = new AiOptions
        {
            Enabled = true,
            Endpoint = new Uri("http://ai.example.org"),
            ApiKey = " ",
            RequestTimeout = TimeSpan.Zero,
            MaxOutputTokens = 0,
            MaxRetries = AiOptionsValidator.MaxRetriesLimit + 1,
        };

        // Act
        var result = _sut.Validate(name: null, options);

        // Assert
        result.Failed.ShouldBeTrue();
        result.Failures.ShouldNotBeNull().Count().ShouldBe(6);
    }

    [Fact(DisplayName = "Unset output limit, temperature and top-p are valid: the provider defaults apply")]
    public void Validate_OptionalSettingsUnset_Succeeds()
    {
        // Arrange
        var options = new AiOptions { Enabled = true, Endpoint = new Uri("https://ai.example.org"), ApiKey = "key", Model = "model" };

        // Act & Assert
        _sut.Validate(name: null, options).Succeeded.ShouldBeTrue();
    }

    [Theory(DisplayName = "Temperature and top-p are checked against their ranges")]
    [InlineData(-0.1f, 0.5f)]
    [InlineData(2.1f, 0.5f)]
    [InlineData(0.5f, -0.1f)]
    [InlineData(0.5f, 1.1f)]
    public void Validate_SamplingOutOfRange_Fails(float temperature, float topP)
    {
        // Arrange
        var options = new AiOptions
        {
            Enabled = true,
            Endpoint = new Uri("https://ai.example.org"),
            ApiKey = "key",
            Model = "model",
            Temperature = temperature,
            TopP = topP,
        };

        // Act & Assert
        _sut.Validate(name: null, options).Failures.ShouldNotBeNull().ShouldHaveSingleItem();
    }

    [Theory(DisplayName = "The MCP timeouts must be positive")]
    [InlineData(0, 60)]
    [InlineData(5, 0)]
    public void Validate_McpTimeouts_NotPositive_Fails(int connectionTimeoutSeconds, int retryIntervalSeconds)
    {
        // Arrange
        var options = new AiOptions
        {
            Enabled = true,
            Endpoint = new Uri("https://ai.example.org"),
            ApiKey = "key",
            Model = "model",
            McpConnectionTimeout = TimeSpan.FromSeconds(connectionTimeoutSeconds),
            McpRetryInterval = TimeSpan.FromSeconds(retryIntervalSeconds),
        };

        // Act & Assert
        _sut.Validate(name: null, options).Failures.ShouldNotBeNull().ShouldHaveSingleItem();
    }

    [Fact(DisplayName = "While AI is disabled the settings are not validated")]
    public void Validate_Disabled_Skipped()
        => _sut.Validate(name: null, new AiOptions { Enabled = false }).Skipped.ShouldBeTrue();

    [Theory(DisplayName = "An MCP server needs an absolute HTTP(S) address, and at least one tool round is allowed")]
    [InlineData("ftp://core.example.org/mcp", 5)]
    [InlineData("https://core.example.org/mcp", 0)]
    public void Validate_InvalidTools_Fails(string endpoint, int maxToolCallRounds)
    {
        // Arrange
        var options = new AiOptions
        {
            Enabled = true,
            Endpoint = new Uri("https://ai.example.org"),
            ApiKey = "key",
            Model = "model",
            MaxToolCallRounds = maxToolCallRounds,
        };
        options.Mcp["core"] = new AiMcpServerOptions { Endpoint = new Uri(endpoint) };

        // Act & Assert
        _sut.Validate(name: null, options).Failures.ShouldNotBeNull().ShouldHaveSingleItem();
    }

    [Fact(DisplayName = "A plain HTTP address of an MCP server is accepted: no secret is sent to it")]
    public void Validate_HttpMcpServer_Succeeds()
    {
        // Arrange
        var options = new AiOptions { Enabled = true, Endpoint = new Uri("https://ai.example.org"), ApiKey = "key", Model = "model" };
        options.Mcp["core"] = new AiMcpServerOptions { Endpoint = new Uri("http://core:8080/mcp") };

        // Act & Assert
        _sut.Validate(name: null, options).Succeeded.ShouldBeTrue();
    }
}
