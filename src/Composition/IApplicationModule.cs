using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Composition;

/// <summary>
/// Application module interface
/// </summary>
public interface IApplicationModule
{
    /// <summary>
    /// Module name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Service modules
    /// </summary>
    IReadOnlyList<ServiceModuleBase> ServiceModules { get; }

    /// <summary>
    /// Configure services for the module
    /// </summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Assemblies associated with the module
    /// </summary>
    Assembly[] Assemblies => ServiceModules
        .Select(x => x.ModuleAssembly)
        .ToArray();
}
