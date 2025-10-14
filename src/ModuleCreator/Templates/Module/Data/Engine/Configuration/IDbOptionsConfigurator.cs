using Microsoft.EntityFrameworkCore;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;

/// <summary>
/// Db options configurator
/// </summary>
public interface IDbOptionsConfigurator
{
    /// <summary>
    /// Setup db options builder
    /// </summary>
    public TBuilder SetupBuilder<TBuilder>(TBuilder builder)
        where TBuilder : DbContextOptionsBuilder;
}
