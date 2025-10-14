using AuroraScienceHub.Framework.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AuroraScienceHub.Framework.AspNetCore.Composition;

/// <summary>
/// Web application module base class.
/// </summary>
/// <remarks>
/// Registers all dependencies and services for the module.
/// </remarks>
public abstract class WebApplicationModuleBase<TModuleOptions>
    : ApplicationModuleBase, IWebApplicationModule
    where TModuleOptions : ApplicationModuleOptionsBase
{
    protected WebApplicationModuleBase(
        string name,
        IReadOnlyList<ServiceModuleBase> serviceModules)
        : base(name, serviceModules)
    {
    }

    /// <inheritdoc />
    public virtual void ConfigureApplication(IApplicationBuilder app, IConfiguration configuration)
    {
    }

    /// <inheritdoc />
    public ApplicationModuleOptionsBase GetModuleOptions(IConfiguration configuration)
    {
        return configuration
                   .GetSection($"{ApplicationModuleOptionsBase.OptionSectionBase}:{Name}")
                   .Get<TModuleOptions>()
               ?? ApplicationModuleOptionsBase.Default;
    }
}
