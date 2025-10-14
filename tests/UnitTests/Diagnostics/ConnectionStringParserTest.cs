using AuroraScienceHub.Framework.Diagnostics;
using DatabaseEngine = AuroraScienceHub.Framework.Diagnostics.ConnectionStringParser.DatabaseEngine;

namespace AuroraScienceHub.Framework.UnitTests.Diagnostics;

/// <summary>
/// Unit tests for <see cref="ConnectionStringParser"/>
/// </summary>
public sealed class ConnectionStringParserTest
{
    [Theory]
    [InlineData(DatabaseEngine.Sqlite, ":memory:", "Data Source=:memory:;Version=3;New=True;")]
    [InlineData(DatabaseEngine.Sqlite, "SWeather", @"Data Source=C:\xxx\yyy\SWeather.db;Version=3;New=True;")]
    [InlineData(DatabaseEngine.Sqlite, "SWeather", @"Data Source=C:/xxx/yyy/SWeather.db;Version=3;New=True;")]
    [InlineData(DatabaseEngine.PostgreSql, "SWeather", "Host=tst-pg-01.com;uid=admin;pwd=42;Port=6432;No Reset On Close=true;Database=SWeather;Maximum Pool Size=512")]
    [InlineData(DatabaseEngine.PostgreSql, "SWeather", "User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=SWeather;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;")]
    [InlineData(DatabaseEngine.MsSql, "SWeather", "server=SpaceWeather.com;Database=SWeather;uid=admin;pwd=42;")]
    [InlineData(DatabaseEngine.MsSql, "SWeather", "Server=myServerAddress;Database=SWeather;User Id=myUsername;Password=myPassword;")]
    [InlineData(DatabaseEngine.Unknown, null, "@-/")]
    [InlineData(DatabaseEngine.Unknown, null, "")]
    public void ShouldParseConnectionString(DatabaseEngine expectedEngine, string? expectedDatabase, string connectionString)
    {
        // Arrange, Act
        var (engine, database) = ConnectionStringParser.Parse(connectionString);

        // Assert
        Assert.Equal(expectedEngine, engine);
        Assert.Equal(expectedDatabase, database);
    }
}
