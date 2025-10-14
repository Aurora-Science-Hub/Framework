using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.InternalClient;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Add{{ModuleName}}InternalClients(this IServiceCollection services)
    {
        return services;
    }
}
