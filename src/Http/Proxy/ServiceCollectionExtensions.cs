using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Http.Proxy;

/// <summary>
/// <see cref="IServiceCollection"/> extensions for configuring HTTP proxy
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds configuration for <see cref="ProxyOptions"/> to the service collection, binding it to the "Proxy" section of the configuration
    /// </summary>
    public static IServiceCollection AddProxyOptions(this IServiceCollection services)
    {
        services.AddOptions<ProxyOptions>()
            .BindConfiguration(ProxyOptions.OptionKey);

        return services;
    }
}
