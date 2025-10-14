using {{CompanyName}}.Framework.EntityFramework.NpgSql;
using Microsoft.Extensions.Options;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;

internal sealed class DbOptionsConfigurator :
    DbOptionsConfiguratorBase<ConnectionOptions, DataContext>,
    IDbOptionsConfigurator
{
    public DbOptionsConfigurator(IOptions<ConnectionOptions> connectionOptions) :
        base(connectionOptions)
    {
    }
}
