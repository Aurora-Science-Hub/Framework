using AuroraScienceHub.Framework.Utilities;
using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities;

/// <summary>
/// Unit tests for <see cref="DateTimeRangeExtensions"/>.
/// </summary>
public sealed class DateTimeRangeExtensionsTests
{
    [Fact]
    public void EnumerateMinutes_ReturnsAllMinutes()
    {
        // Arrange
        var start = new DateTime(2021, 1, 1, 0, 0, 0);
        var end = new DateTime(2021, 1, 1, 0, 2, 0);
        var range = new DateTimeRange(start, end);

        // Act
        var minutes = range.EnumerateMinutes().ToArray();

        // Assert
        minutes.ShouldBeEquivalentTo(new[]
        {
            new DateTime(2021, 1, 1, 0, 0, 0),
            new DateTime(2021, 1, 1, 0, 1, 0),
            new DateTime(2021, 1, 1, 0, 2, 0)
        });
    }

    [Fact]
    public void EnumerateHours_ReturnsAllHours()
    {
        // Arrange
        var start = new DateTime(2021, 1, 1, 0, 0, 0);
        var end = new DateTime(2021, 1, 1, 5, 2, 0);
        var range = new DateTimeRange(start, end);

        // Act
        var minutes = range.EnumerateHours().ToArray();

        // Assert
        minutes.ShouldBeEquivalentTo(new[]
        {
            new DateTime(2021, 1, 1, 0, 0, 0),
            new DateTime(2021, 1, 1, 1, 0, 0),
            new DateTime(2021, 1, 1, 2, 0, 0),
            new DateTime(2021, 1, 1, 3, 0, 0),
            new DateTime(2021, 1, 1, 4, 0, 0),
            new DateTime(2021, 1, 1, 5, 0, 0)
        });
    }
}
