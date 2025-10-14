{{ if AddDbContext == true }}
using {{CompanyName}}.Framework.Composition;
using {{CompanyName}}.Framework.Configuration;
using {{CompanyName}}.Framework.Diagnostics.HealthChecks;
using {{CompanyName}}.Framework.Exceptions;
using {{CompanyName}}.Framework.EntityFramework.Interceptors;
using {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine;
using {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data;

public sealed class ServiceModule : ServiceModuleBase
{
    protected override void ConfigureServicesInternal(IServiceCollection services, IConfiguration configuration)
    {
        AddEngine(services, configuration);
        AddInterceptors(services);
    }

    private static void AddEngine(IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguredOptions<ConnectionOptions>();

        services.AddTransient<IDbOptionsConfigurator, DbOptionsConfigurator>();
        services.AddTransient(typeof(DbContextOptionsFactory));

        services.AddDbContext<DataContext>((provider, builder) =>
        {
            provider.GetRequiredService<DbContextOptionsFactory>().SetupOptions(builder);
        }, optionsLifetime: ServiceLifetime.Singleton);

        var connection = configuration.GetRequiredOptions<ConnectionOptions>();

        var connectionString = UnexpectedException.ThrowIfNull(
            connection.SpaceWeather,
            $"Connection string '{nameof(ConnectionOptions.SpaceWeather)}' is not configured");

        services.AddNonDuplicateHealthChecks().CheckPostgreSql(connectionString);
    }

    private static void AddInterceptors(IServiceCollection services)
    {
        services.AddTransientAsImplementedInterfaces<AuditableInterceptor>();
        services.AddTransientAsImplementedInterfaces<DeletableInterceptor>();
    }
}
{{ else }}
using {{CompanyName}}.Framework.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data;

public sealed class ServiceModule : ServiceModuleBase
{
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add your services here
    }
}
{{ end }}
