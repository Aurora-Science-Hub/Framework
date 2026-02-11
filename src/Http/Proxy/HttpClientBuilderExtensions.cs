using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Http.Proxy;

/// <summary>
/// <see cref="IHttpClientBuilder"/> extensions for configuring HTTP proxy
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Configures the primary HTTP message handler to use a proxy if configured in <see cref="ProxyOptions"/>
    /// </summary>
    public static IHttpClientBuilder ConfigurePrimaryHttpProxyMessageHandler(this IHttpClientBuilder builder)
    {
        builder.ConfigurePrimaryHttpMessageHandler(serviceProvider =>
        {
            var proxyOptions = serviceProvider
                .GetRequiredService<IOptions<ProxyOptions>>()
                .Value;

            if (proxyOptions.Address is null)
            {
                throw new InvalidOperationException(
                    $"Proxy address is not configured for section {ProxyOptions.OptionKey}");
            }

            var credentialsProvided = !string.IsNullOrWhiteSpace(proxyOptions.UserName) &&
                                      !string.IsNullOrEmpty(proxyOptions.Password);

            var handler = new HttpClientHandler
            {
                UseProxy =  true,
                Proxy = new WebProxy
                {
                    Address = proxyOptions.Address,
                    BypassProxyOnLocal = true,
                    UseDefaultCredentials = false,
                    Credentials = credentialsProvided
                        ? new NetworkCredential(proxyOptions.UserName, proxyOptions.Password)
                        : null,
                },
            };

            return handler;
        });

        return builder;
    }
}
