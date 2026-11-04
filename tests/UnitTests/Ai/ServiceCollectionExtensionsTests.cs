using AuroraScienceHub.Framework.Ai;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    private const string ApiKey = "test-secret-key";

    [Fact(DisplayName = "While AI is disabled the services are registered and the provider settings are not validated")]
    public async Task AddAi_Disabled_RegisteredWithoutValidation()
    {
        // Arrange
        await using var provider = BuildProvider(new Dictionary<string, string?> { ["Ai:Enabled"] = "false" });

        // Act
        var options = provider.GetRequiredService<IOptions<AiOptions>>().Value;

        // Assert
        options.Enabled.ShouldBeFalse();
        var registrations = provider.GetRequiredService<IServiceProviderIsService>();
        registrations.IsService(typeof(IAiAssistant)).ShouldBeTrue();
        registrations.IsService(typeof(IConversationCompactor)).ShouldBeTrue();
    }

    [Fact(DisplayName = "The assistant and the compactor are resolved when AI is enabled and configured")]
    public async Task AddAi_Enabled_ResolvesServices()
    {
        // Arrange
        await using var provider = BuildProvider(ValidSettings());

        // Act & Assert
        provider.GetRequiredService<IAiAssistant>().ShouldNotBeNull();
        provider.GetRequiredService<IConversationCompactor>().ShouldNotBeNull();
    }

    [Theory(DisplayName = "Invalid settings fail validation without revealing the API key")]
    [InlineData("Ai:Endpoint", "not a uri")]
    [InlineData("Ai:Endpoint", "ftp://example.org")]
    [InlineData("Ai:Endpoint", "http://ai.example.org")]
    [InlineData("Ai:Model", "")]
    [InlineData("Ai:ApiKey", "")]
    [InlineData("Ai:RequestTimeout", "00:00:00")]
    [InlineData("Ai:MaxRetries", "6")]
    [InlineData("Ai:MaxOutputTokens", "0")]
    public async Task AddAi_InvalidSettings_FailValidation(string key, string value)
    {
        // Arrange
        var settings = ValidSettings();
        settings[key] = value;
        await using var provider = BuildProvider(settings);

        // Act
        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<AiOptions>>().Value);

        // Assert
        exception.Message.ShouldNotContain(ApiKey);
    }

    [Fact(DisplayName = "Plain HTTP is accepted only for a local model on a loopback address")]
    public async Task AddAi_HttpLoopback_Accepted()
    {
        // Arrange
        var settings = ValidSettings();
        settings["Ai:Endpoint"] = "http://localhost:11434/v1";
        await using var provider = BuildProvider(settings);

        // Act & Assert
        provider.GetRequiredService<IOptions<AiOptions>>().Value.Endpoint.ShouldNotBeNull().IsLoopback.ShouldBeTrue();
    }

    [Fact(DisplayName = "The standard resilience handler a host adds to every client is removed from the provider client")]
    public async Task AddAi_HostResilience_Removed()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(ValidSettings()).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());
        services.AddAi(configuration);
        await using var provider = services.BuildServiceProvider();

        // Act
        var handlers = new List<HttpMessageHandler>();
        for (HttpMessageHandler? handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(ServiceCollectionExtensions.HttpClientName);
             handler is not null;
             handler = (handler as DelegatingHandler)?.InnerHandler)
        {
            handlers.Add(handler);
        }

        // Assert
        handlers.ShouldNotContain(handler => handler.GetType().Name.Contains("Resilience", StringComparison.Ordinal));
    }

    private static Dictionary<string, string?> ValidSettings()
        => new()
        {
            ["Ai:Enabled"] = "true",
            ["Ai:Endpoint"] = "https://ai.example.org",
            ["Ai:ApiKey"] = ApiKey,
            ["Ai:Model"] = "test-model",
        };

    private static ServiceProvider BuildProvider(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddAi(configuration);
        return services.BuildServiceProvider();
    }
}
