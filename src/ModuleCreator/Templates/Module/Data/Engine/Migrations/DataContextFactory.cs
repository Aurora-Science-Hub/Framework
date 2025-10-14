using {{CompanyName}}.Framework.EntityFramework.NpgSql;
using {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine.Migrations;

public sealed class DataContextFactory : PostgreSqlDbContextFactoryBase<DataContext, ConnectionOptions>
{
    protected override DataContext CreateDbContext(DbContextOptions<DataContext> options)
    {
        return new DataContext(options, Array.Empty<IInterceptor>());
    }
}
