using System.Net;
using Ai.Proxy;
using AuroraScienceHub.Framework.Utilities.System;
using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ai.Gpt;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGptClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GptOptions>()
            .BindConfiguration(GptOptions.OptionKey);

        services.AddOptions<ProxyOptions>()
            .BindConfiguration(ProxyOptions.OptionKey);

        services.AddTransient<IGptClient, GptClient>();

        services.AddChatGpt((serviceProvider, options) =>
        {
            var gptOptions = serviceProvider.GetRequiredService<IOptions<GptOptions>>().Value;

            // OpenAI.
            options.UseOpenAI(apiKey: gptOptions.ApiKey.Required(), organization: string.Empty);

            options.DefaultModel = gptOptions.Model.Required();
            options.MessageLimit = gptOptions.MessageLimit;
            options.MessageExpiration = gptOptions.MessageExpiration;
            options.DefaultParameters = new ChatGptParameters
            {
                MaxTokens = 4096,
                Temperature = 0.2
            };
        }, ConfigureHttpClient);

        return services;

        void ConfigureHttpClient(IHttpClientBuilder httpClientBuilder)
        {
            var gptOptions = configuration.GetSection(GptOptions.OptionKey)
                .Get<GptOptions>()
                .Required();
            if (gptOptions.UseProxy == false)
            {
                return;
            }

            var proxyOptions = configuration.GetSection(ProxyOptions.OptionKey)
                .Get<ProxyOptions>()
                .Required();

            // Create the handler with proxy settings
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
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            httpClientBuilder.ConfigureHttpClient(client =>
            {
                client.Timeout = gptOptions.Timeout;
            });

            // Configure the HttpClient to use this handler
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => handler);
        }
    }
}
