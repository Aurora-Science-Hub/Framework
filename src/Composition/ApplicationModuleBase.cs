using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.Composition;

/// <summary>
/// Base class for application module.
/// </summary>
/// <remarks>
/// Combines and registers multiple service modules that are part of the application.
/// </remarks>
public abstract class ApplicationModuleBase : IApplicationModule
{
    /// <summary>
    /// ctor
    /// </summary>
    protected ApplicationModuleBase(
        string name,
        params IReadOnlyList<ServiceModuleBase> serviceModules)
    {
        Name = name;
        ServiceModules = serviceModules;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyList<ServiceModuleBase> ServiceModules { get; }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        foreach (var serviceModule in ServiceModules)
        {
            serviceModule.ConfigureServices(services, configuration);
        }
    }

    /// <summary>
    /// Create service module list from specified types
    /// </summary>
    protected static IReadOnlyList<ServiceModuleBase> From<T0, T1, T2, T3>()
        where T0 : ServiceModuleBase, new()
        where T1 : ServiceModuleBase, new()
        where T2 : ServiceModuleBase, new()
        where T3 : ServiceModuleBase, new()
        => [new T0(), new T1(), new T2(), new T3()];
}
