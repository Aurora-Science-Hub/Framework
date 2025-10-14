using System.Net;
using Ai.Proxy;
using AuroraScienceHub.Framework.Utilities.System;
using DeepL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ai.Deepl;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeepl(this IServiceCollection services)
    {
        services.AddOptions<DeeplOptions>()
            .BindConfiguration(DeeplOptions.OptionKey);

        services.AddHttpClient("DeeplProxy")
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var proxyOptions = serviceProvider.GetRequiredService<IOptions<ProxyOptions>>().Value;
                return new HttpClientHandler
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
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };
            });

        services.AddTransient<Translator>(serviceProvider =>
        {
            var deeplOptions = serviceProvider.GetRequiredService<IOptions<DeeplOptions>>().Value;
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            DeepLClientOptions clientOptions;
            if (!deeplOptions.UseProxy)
            {
                clientOptions = new DeepLClientOptions { sendPlatformInfo = false };
            }
            else
            {
                clientOptions = new DeepLClientOptions
                {
                    ClientFactory = () => new HttpClientAndDisposeFlag
                    {
                        HttpClient = httpClientFactory.CreateClient("DeeplProxy"),
                        DisposeClient = false,
                    },
                    sendPlatformInfo = false,
                };
            }

            return new Translator(deeplOptions.ApiKey.Required(), clientOptions);
        });

        services.AddTransient<IDeeplClient, DeeplClient>();

        return services;
    }
}
