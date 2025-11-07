using System.ClientModel;
using System.ClientModel.Primitives;
using System.Net;
using AuroraScienceHub.Framework.Ai.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        services.AddOptions();
        services.ConfigureHttpClient();
        services.ConfigureGptClient();

        services.AddTransient<IGptClient, GptClient>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<GptOptions>()
            .BindConfiguration(GptOptions.OptionKey);

        services.AddOptions<ProxyOptions>()
            .BindConfiguration(ProxyOptions.OptionKey);

        return services;
    }

    private static void ConfigureHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient(GptClientHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var gptOptions = serviceProvider
                    .GetRequiredService<IOptions<GptOptions>>()
                    .Value;

                if (gptOptions.UseProxy == false)
                {
                    return new HttpClientHandler();
                }

                var proxyOptions = serviceProvider
                    .GetRequiredService<IOptions<ProxyOptions>>()
                    .Value;

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
    }

    private static void ConfigureGptClient(this IServiceCollection services)
    {
        services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var gptOptions = serviceProvider
                .GetRequiredService<IOptions<GptOptions>>()
                .Value;

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(GptClientHttpClientName);

            var client = new ChatClient(
                model: gptOptions.RequiredModel,
                credential: new ApiKeyCredential(gptOptions.RequiredApiKey),
                options: new OpenAIClientOptions()
                {
                    Endpoint = gptOptions.Endpoint, Transport = new HttpClientPipelineTransport(httpClient),
                });

            return client;
        });
    }
}
