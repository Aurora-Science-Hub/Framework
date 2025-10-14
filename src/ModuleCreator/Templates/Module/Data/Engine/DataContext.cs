using {{CompanyName}}.Framework.EntityFramework;
using {{CompanyName}}.Framework.EntityFramework.Converters.Identifiers;
using {{CompanyName}}.Framework.EntityFramework.NpgSql.Conventions;
using {{CompanyName}}.Framework.EntityFramework.Converters.DateTimes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace {{CompanyName}}.{{ServiceName}}.{{ModuleName}}.Data.Engine;

public sealed class DataContext : DbContext, IDataContext
{
    private const string Schema = "rdi";

    static string IDataContext.Schema => Schema;

    private readonly IReadOnlyList<IInterceptor> _interceptors;

    public DataContext(
        DbContextOptions<DataContext> options,
        IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        _interceptors = interceptors.ToList();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(_interceptors)
            .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseIdentifierConversion();
        modelBuilder.UseDateTimeToUtcConversion();
        modelBuilder.UseDateTimeOffsetToUtcConversion();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.HasDefaultCollation();
    }
}
