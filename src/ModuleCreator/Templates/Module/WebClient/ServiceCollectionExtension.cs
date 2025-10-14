using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.WebClient;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Add{{ModuleName}}WebClients(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<{{ModuleName}}ClientOptions>? configureOptions = null)
    {
        services.Configure<{{ModuleName}}ClientOptions>(configuration.GetSection({{ModuleName}}ClientOptions.OptionKey));
        services.Configure(configureOptions ?? (_ => { }));

        //services.AddHttpClient<ISomeResourceClient, SomeResourceWebClient>();

        return services;
    }
}
