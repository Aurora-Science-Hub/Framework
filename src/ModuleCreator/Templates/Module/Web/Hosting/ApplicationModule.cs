using {{CompanyName}}.Framework.AspNetCore.Composition;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Web.Hosting;

public sealed class Module : WebApplicationModuleBase<ModuleOptions>
{
    public Module()
        : base("{{ModuleName}}", From<Domain.ServiceModule, Application.ServiceModule, Data.ServiceModule, Web.ServiceModule>())
    {
    }
}
