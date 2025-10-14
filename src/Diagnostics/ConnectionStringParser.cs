using System.Text.RegularExpressions;

namespace AuroraScienceHub.Framework.Diagnostics;

/// <summary>
/// Detects the type of the used database by its connection string.
/// </summary>
/// <remarks>
/// Accuracy of detection is quite weak. It is not recommended to make critical conclusions based on this information.
/// </remarks>
public static class ConnectionStringParser
{
    public enum DatabaseEngine
    {
        Unknown,
        PostgreSql,
        MsSql,
        Sqlite,
    }

    private static readonly Regex s_msSqlRegex = new(@"Database\s*=\s*(?<dbName>[^;]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_postgreSqlRegex = new(@"Database\s*=\s*(?<dbName>[^;]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_sqliteRegex = new(@"Data Source\s*=\s*(?:.*[\\/])?(?<dbName>[^;]+?)(?:\.db)?(?:;|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses the connection string to determine the database engine and database name.
    /// </summary>
    /// <param name="connectionString">The connection string to parse.</param>
    /// <returns>A tuple containing the database engine and the database name.</returns>
    public static (DatabaseEngine Engine, string? Database) Parse(string connectionString)
    {
        var engine = ParseEngine(connectionString);
        var databaseName = ParseDatabaseName(connectionString, engine);
        return (engine, databaseName);
    }

    private static DatabaseEngine ParseEngine(string connectionString)
    {
        if (connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("server=", StringComparison.OrdinalIgnoreCase))
        {
            if (connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains(".db", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("SQLite", StringComparison.OrdinalIgnoreCase))
            {
                return DatabaseEngine.Sqlite;
            }
            return DatabaseEngine.MsSql;
        }
        else if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseEngine.PostgreSql;
        }

        return DatabaseEngine.Unknown;
    }

    private static string? ParseDatabaseName(string connectionString, DatabaseEngine engine)
    {
        Regex? regex = engine switch
        {
            DatabaseEngine.MsSql => s_msSqlRegex,
            DatabaseEngine.PostgreSql => s_postgreSqlRegex,
            DatabaseEngine.Sqlite => s_sqliteRegex,
            _ => null
        };

        if (regex == null)
        {
            return null;
        }

        var match = regex.Match(connectionString);
        return match.Success ? match.Groups["dbName"].Value : null;
    }
}
