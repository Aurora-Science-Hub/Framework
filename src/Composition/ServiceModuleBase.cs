using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Composition;

/// <summary>
/// Base class for the service module, which contains services configuration logic.
/// </summary>
/// <remarks>
/// It is also guaranteed that the services will be configured only once for each module.
/// </remarks>
public abstract class ServiceModuleBase
{
    /// <summary>
    /// Assembly associated with current module
    /// </summary>
    public Assembly ModuleAssembly => GetType().Assembly;

    /// <summary>
    /// Configure services for the module
    /// </summary>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        if (!services.TryAddMarker(GetType()))
        {
            return;
        }

        ConfigureServicesInternal(services, configuration);
    }

    /// <summary>
    /// Configure services for the module
    /// </summary>
    protected abstract void ConfigureServicesInternal(IServiceCollection services, IConfiguration configuration);
}
