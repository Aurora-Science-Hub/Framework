using AuroraScienceHub.Framework.Diagnostics.HealthChecks;
using DatabaseEngine = AuroraScienceHub.Framework.Diagnostics.ConnectionStringParser.DatabaseEngine;

namespace AuroraScienceHub.Framework.UnitTests.Diagnostics;

/// <summary>
/// Unit tests for <see cref="HealthChecksExtensions"/>
/// </summary>
public sealed class HealthChecksExtensionTest
{
    [Theory]
    [InlineData(DatabaseEngine.PostgreSql, "SomeDb", "SomeDb PostgreSql")]
    [InlineData(DatabaseEngine.Unknown, "SomeDb", "SomeDb Database")]
    [InlineData(DatabaseEngine.Unknown, null, "Database")]
    public void ShouldFormatConnectionString(DatabaseEngine engine, string? database, string expectedFormat)
    {
        // Act
        var actualFormat = HealthChecksExtensions.FormatDisplayName(engine, database);

        // Assert
        Assert.Equal(expectedFormat, actualFormat);
    }
}
