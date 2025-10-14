using Microsoft.EntityFrameworkCore;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;

internal sealed class DbContextOptionsFactory
{
    private readonly IDbOptionsConfigurator _optionsConfigurator;

    public DbContextOptionsFactory(IDbOptionsConfigurator optionsConfigurator)
    {
        _optionsConfigurator = optionsConfigurator;
    }

    public DbContextOptionsBuilder SetupOptions(DbContextOptionsBuilder options)
    {
        _optionsConfigurator.SetupBuilder(options);
        return options;
    }
}
