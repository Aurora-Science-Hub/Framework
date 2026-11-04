using System.Net;
using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Agents.AI;
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
    private const string ProxyAddress = "http://proxy.example.org:3128";

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

    [Fact(DisplayName = "The real graph is resolved while AI is disabled without a key and a model, and the provider client is not created")]
    public async Task AddAi_DisabledWithoutKeyAndModel_ResolvesGraph()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Ai:Enabled"] = "false" })
            .Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddAi(configuration);

        // Act
        await using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });

        // Assert
        provider.GetRequiredService<IAiAssistant>().ShouldNotBeNull();
        provider.GetRequiredService<IConversationCompactor>().ShouldNotBeNull();
        provider.GetRequiredService<AIAgent>().ShouldNotBeNull();
        provider.GetRequiredService<IChatClient>().ShouldBeOfType<DeferredChatClient>();
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

    [Fact(DisplayName = "The proxy the caller enables through the options is used by the provider and MCP clients")]
    public async Task AddAi_UseProxyFromConfigure_ProxiedClientsAreUsed()
    {
        // Arrange
        var settings = ValidSettings();
        settings["Proxy:Address"] = ProxyAddress;
        await using var provider = BuildProvider(settings, options => options.UseProxy = true);
        var options = provider.GetRequiredService<IOptions<AiOptions>>().Value;

        // Act
        var providerHandler = CreatePrimaryHandler(
            provider,
            ServiceCollectionExtensions.GetProviderHttpClientName(options));
        var mcpHandler = CreatePrimaryHandler(
            provider,
            ServiceCollectionExtensions.GetMcpHttpClientName(options));

        // Assert
        providerHandler.ShouldBeOfType<HttpClientHandler>()
            .Proxy.ShouldBeOfType<WebProxy>()
            .Address.ShouldBe(new Uri(ProxyAddress));
        mcpHandler.ShouldBeOfType<HttpClientHandler>().Proxy.ShouldNotBeNull();
    }

    [Fact(DisplayName = "The primary handler the host configures stays for the direct clients")]
    public async Task AddAi_ProxyOff_HostPrimaryHandlerKept()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(ValidSettings()).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.ConfigureHttpClientDefaults(http => http.UseSocketsHttpHandler(
            (handler, _) => handler.PooledConnectionLifetime = TimeSpan.FromMinutes(5)));
        services.AddAi(configuration);
        await using var provider = services.BuildServiceProvider();

        // Act
        var handler = CreatePrimaryHandler(provider, ServiceCollectionExtensions.HttpClientName);

        // Assert
        handler.ShouldBeOfType<SocketsHttpHandler>();
    }

    [Fact(DisplayName = "Changes of the caller are applied after the configuration is bound")]
    public async Task AddAi_Configure_AppliedAfterBinding()
    {
        // Arrange
        await using var provider = BuildProvider(ValidSettings(), options => options.MaxOutputTokens = 2000);

        // Act
        var options = provider.GetRequiredService<IOptions<AiOptions>>().Value;

        // Assert
        options.Model.ShouldBe("test-model");
        options.MaxOutputTokens.ShouldBe(2000);
    }

    [Fact(DisplayName = "A tool provider of the service is used together with the MCP provider")]
    public async Task AddAi_CustomToolProvider_RegisteredAlongsideMcp()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(ValidSettings()).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddAi(configuration);
        services.AddSingleton<IAiToolProvider>(new EmptyToolProvider());
        await using var provider = services.BuildServiceProvider();

        // Act
        var toolProviders = provider.GetServices<IAiToolProvider>().ToList();

        // Assert
        toolProviders.Select(toolProvider => toolProvider.GetType())
            .ShouldBe([typeof(McpToolProvider), typeof(EmptyToolProvider)]);
    }

    /// <summary>
    /// The handler of the client without the wrappers of the factory, which track the handler lifetime
    /// </summary>
    private static HttpMessageHandler CreatePrimaryHandler(IServiceProvider provider, string clientName)
    {
        var handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(clientName);
        while (handler is DelegatingHandler { InnerHandler: { } innerHandler })
        {
            handler = innerHandler;
        }

        return handler;
    }

    private static Dictionary<string, string?> ValidSettings()
        => new()
        {
            ["Ai:Enabled"] = "true",
            ["Ai:Endpoint"] = "https://ai.example.org",
            ["Ai:ApiKey"] = ApiKey,
            ["Ai:Model"] = "test-model",
        };

    private static ServiceProvider BuildProvider(Dictionary<string, string?> settings, Action<AiOptions>? configure = null)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddAi(configuration, configure);
        return services.BuildServiceProvider();
    }

    private sealed class EmptyToolProvider : IAiToolProvider
    {
        public Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<AITool>>([]);
    }
}
