using AuroraScienceHub.Framework.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AuroraScienceHub.Framework.AspNetCore.Composition;

/// <summary>
/// Represents a web application module.
/// </summary>
public interface IWebApplicationModule : IApplicationModule
{
    /// <summary>
    /// Configures the application
    /// </summary>
    void ConfigureApplication(IApplicationBuilder app, IConfiguration configuration);

    /// <summary>
    /// Get module options
    /// </summary>
    ApplicationModuleOptionsBase GetModuleOptions(IConfiguration configuration);
}
