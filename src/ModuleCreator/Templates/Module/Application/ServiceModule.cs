using {{CompanyName}}.Framework.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Application;

public sealed class ServiceModule : ServiceModuleBase
{
    protected override void ConfigureServicesInternal(IServiceCollection services, IConfiguration configuration) { }
}
