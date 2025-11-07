using System.ClientModel;
using System.ClientModel.Primitives;
using System.Net;
using AuroraScienceHub.Framework.Ai.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai.Gpt;

/// <summary>
/// <see cref="IServiceCollection"/> extensions for GPT client
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string GptClientHttpClientName = "GptClient";

    /// <summary>
    /// Add GPT client
    /// </summary>
    public static IServiceCollection AddGptClient(this IServiceCollection services, IConfiguration configuration)
    {
        var proxyOptions = configuration.GetSection(ProxyOptions.OptionKey)
            .Get<ProxyOptions>() ?? new ProxyOptions();
        var gptOptions = configuration.GetSection(GptOptions.OptionKey)
            .Get<GptOptions>() ?? throw new InvalidOperationException("GPT options are not configured.");

        services.AddOptions<GptOptions>()
            .BindConfiguration(GptOptions.OptionKey);

        services.AddOptions<ProxyOptions>()
            .BindConfiguration(ProxyOptions.OptionKey);

        services.ConfigureHttpClient(gptOptions, proxyOptions);
        services.ConfigureGptClient(gptOptions);

        services.AddTransient<IGptClient, GptClient>();

        return services;
    }

    private static IServiceCollection ConfigureHttpClient(
        this IServiceCollection services,
        GptOptions gptOptions,
        ProxyOptions proxyOptions)
    {
        if (gptOptions.UseProxy == false)
        {
            services.AddHttpClient(GptClientHttpClientName);
            return services;
        }

        services.AddHttpClient(GptClientHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy
                    {
                        Address = proxyOptions.Address,
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(
                            userName: proxyOptions.UserName,
                            password: proxyOptions.Password),
                    },
                };
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });

        return services;
    }

    private static IServiceCollection ConfigureGptClient(
        this IServiceCollection services,
        GptOptions gptOptions)
    {
        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(GptClientHttpClientName);

            var client = new ChatClient(
                model: gptOptions.RequiredModel,
                credential: new ApiKeyCredential(gptOptions.RequiredApiKey),
                options: new OpenAIClientOptions()
                {
                    Endpoint = gptOptions.Endpoint, Transport = new HttpClientPipelineTransport(httpClient),
                });

            return client;
        });

        return services;
    }
}
