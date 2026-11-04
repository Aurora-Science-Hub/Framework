using System.ClientModel;
using System.ClientModel.Primitives;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using AuroraScienceHub.Framework.Http.Proxy;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Registration of the language model provider and the assistant
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Name of the HTTP client of the provider
    /// </summary>
    public const string HttpClientName = "AiProvider";

    /// <summary>
    /// Name of the HTTP client of the MCP servers
    /// </summary>
    public const string McpHttpClientName = "AiMcp";

    /// <summary>
    /// Suffix of the name of the clients that go through the proxy
    /// </summary>
    private const string ProxyClientSuffix = "Proxy";

    /// <summary>
    /// Registers <see cref="IAiAssistant"/> and <see cref="IConversationCompactor"/> backed by the provider configured
    /// in the <see cref="AiOptions.OptionKey"/> section
    /// </summary>
    /// <remarks>
    /// The services are registered even while the AI features are disabled, but they are singletons: the provider client
    /// is created when the service graph is resolved, which a hosted service does on start, not on the first request.
    /// Creating it sends nothing, yet the SDK validates the key in its constructor, so a disabled assistant still needs
    /// a placeholder key; callers check <see cref="AiOptions.Enabled"/> before they ask. <paramref name="configure"/> is
    /// registered as an options change, so it runs again when the configuration is reloaded and keeps its changes.
    /// </remarks>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="configure">Changes of the options applied after the configuration is bound</param>
    public static IServiceCollection AddAi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AiOptions>? configure = null)
    {
        services.AddOptions<AiOptions>()
            .Bind(configuration.GetSection(AiOptions.OptionKey))
            .ValidateOnStart();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<AiOptions>, AiOptionsValidator>());

        services.AddProxyOptions();
        ConfigureAiClient(services.AddHttpClient(HttpClientName));

        // Tool calls run within the time budget of the request as well
        ConfigureAiClient(services.AddHttpClient(McpHttpClientName));

        // A proxied request needs another primary handler, and the host sets one of its own for every client (a pooled
        // sockets handler whose lifetime it keeps). So the proxy has clients of their own, and the bound options choose
        // one when a service is resolved: the `configure` delegate counts as well, unlike a check of the configuration
        // here at registration. The proxied clients are created on first use, so a service with the proxy off never
        // reads the proxy address
        ConfigureAiClient(services.AddHttpClient(HttpClientName + ProxyClientSuffix))
            .ConfigurePrimaryHttpProxyMessageHandler();
        ConfigureAiClient(services.AddHttpClient(McpHttpClientName + ProxyClientSuffix))
            .ConfigurePrimaryHttpProxyMessageHandler();

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton(CreateChatClient);
        services.TryAddSingleton(CreateAgent);
        services.TryAddSingleton<AiRequestExecutor>();
        // Every implementation is used; a service may add its own provider for an MCP server that needs authentication
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IAiToolProvider, McpToolProvider>());
        services.TryAddSingleton<IAiAssistant, AiAssistant>();
        services.TryAddSingleton<IConversationCompactor, ConversationCompactor>();

        return services;
    }

    /// <summary>
    /// Name of the client of the provider that <paramref name="options"/> select
    /// </summary>
    internal static string GetProviderHttpClientName(AiOptions options)
        => options.UseProxy ? HttpClientName + ProxyClientSuffix : HttpClientName;

    /// <summary>
    /// Name of the client of the MCP servers that <paramref name="options"/> select
    /// </summary>
    internal static string GetMcpHttpClientName(AiOptions options)
        => options.UseProxy ? McpHttpClientName + ProxyClientSuffix : McpHttpClientName;

    /// <summary>
    /// Prepares a client of the assistant
    /// </summary>
    /// <remarks>
    /// A host may add the standard resilience handler to every HttpClient (Host.ServiceDefaults does). Remove it: its
    /// 30-second timeouts would cut long answers, and its retries would add to those of the SDK. The SDK limits each
    /// attempt (NetworkTimeout), the executor the whole request. The timeout of the client is set to infinite for the
    /// same reason. <c>RemoveAllResilienceHandlers</c> is the only way to remove handlers added by
    /// <c>ConfigureHttpClientDefaults</c>, but it is still marked experimental
    /// </remarks>
    private static IHttpClientBuilder ConfigureAiClient(IHttpClientBuilder builder)
    {
#pragma warning disable EXTEXP0001
        builder
            .RemoveAllResilienceHandlers()
            .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan);
#pragma warning restore EXTEXP0001

        return builder;
    }

    private static IChatClient CreateChatClient(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<AiOptions>>().Value;

        // The factory supports one provider kind; a new one needs a client implementation here
        if (options.Provider != AiProvider.OpenAICompatible)
        {
            throw new AiProviderNotSupportedException(options.Provider);
        }

        var httpClient = provider.GetRequiredService<IHttpClientFactory>()
            .CreateClient(GetProviderHttpClientName(options));
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        // Validated when AI is enabled: the endpoint, the key and the model are set
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = options.Endpoint,
            Transport = new HttpClientPipelineTransport(httpClient),
            NetworkTimeout = options.RequestTimeout,
            RetryPolicy = new ClientRetryPolicy(options.MaxRetries),
        };
        var chatClient = new OpenAIClient(new ApiKeyCredential(options.ApiKey ?? string.Empty), clientOptions)
            .GetChatClient(options.Model)
            .AsIChatClient();

        // Function invocation is outermost, so every model call of a tool round is traced and logged on its own
        return new ChatClientBuilder(chatClient)
            .UseFunctionInvocation(
                loggerFactory,
                invoking => invoking.MaximumIterationsPerRequest = options.MaxToolCallRounds)
            .UseOpenTelemetry(loggerFactory, AiTelemetry.SourceName)
            .UseLogging(loggerFactory)
            .Build(provider);
    }

    private static AIAgent CreateAgent(IServiceProvider provider)
        => provider.GetRequiredService<IChatClient>().AsAIAgent(
            new ChatClientAgentOptions { Name = "assistant" },
            provider.GetRequiredService<ILoggerFactory>(),
            provider);
}
