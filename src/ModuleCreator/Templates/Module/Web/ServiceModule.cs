using {{CompanyName}}.Framework.Composition;
using {{CompanyName}}.Framework.Configuration;
using {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Web.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Web;

public sealed class ServiceModule : ServiceModuleBase
{
    protected override void ConfigureServicesInternal(IServiceCollection services, IConfiguration configuration)
    {
        base.ConfigureServices(services, configuration);

        services.AddConfiguredOptions<ModuleOptions>();
        ConfigureJobs(services);
    }

    private static void ConfigureJobs(IServiceCollection services)
    {
        // Add your hosted services here
    }
}
