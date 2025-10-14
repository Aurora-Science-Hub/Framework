using AuroraScienceHub.Framework.Utilities.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Configuration;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register options with configuration binding and configuration action
    /// </summary>
    public static void AddConfiguredOptions<T>(this IServiceCollection services, Action<T>? configureOptions = null)
        where T : class, IOptionDescription
        => services
            .AddOptions<T>()
            .BindConfiguration(T.OptionKey)
            .Configure(configureOptions ?? (_ => { }));

    /// <summary>
    /// Register <see cref="HostOptions"/>
    /// </summary>
    public static void AddHostOptions(this IServiceCollection services)
    {
        services.AddConfiguredOptions<HostOptions>();
    }
}
