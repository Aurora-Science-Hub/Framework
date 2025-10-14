using Microsoft.Extensions.DependencyInjection;
using DatabaseEngine = AuroraScienceHub.Framework.Diagnostics.ConnectionStringParser.DatabaseEngine;

namespace AuroraScienceHub.Framework.Diagnostics.HealthChecks;

public static class HealthChecksExtensions
{
    /// <summary>
    /// Add health checks with duplicate registration prevention
    /// </summary>
    public static IHealthChecksBuilder AddNonDuplicateHealthChecks(this IServiceCollection services)
    {
        return new NonDuplicateHealthChecksBuilder(services.AddHealthChecks());
    }

    /// <summary>
    /// Add health checks for PostgreSQL database
    /// </summary>
    public static IHealthChecksBuilder CheckPostgreSql(this IHealthChecksBuilder builder, string connectionString)
    {
        return builder.AddNpgSql(
            name: ResolveDisplayName(connectionString),
            connectionString: connectionString,
            tags: ["database"]);
    }

    private static string ResolveDisplayName(string connectionString)
    {
        var (engine, database) = ConnectionStringParser.Parse(connectionString);
        return FormatDisplayName(engine, database);
    }

    internal static string FormatDisplayName(DatabaseEngine engine, string? database)
    {
        var engineText = engine == DatabaseEngine.Unknown ? "Database" : engine.ToString();
        return $"{database} {engineText}".Trim();
    }
}
