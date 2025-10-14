using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="DateTimeExtensions"/>.
/// </summary>
public class DateTimeExtensionsTests
{
    [Fact]
    public void EnumerateTo_WhenMinuteInterval_ReturnsCorrectSequence()
    {
        // Arrange
        var start = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2022, 1, 1, 0, 2, 0, DateTimeKind.Utc);

        // Act
        var result = start.EnumerateTo(end, TimeSpan.FromMinutes(1)).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result.ShouldBeInOrder();
        result.ShouldBe(new[]
        {
            new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 1, 1, 0, 1, 0, DateTimeKind.Utc),
            new DateTime(2022, 1, 1, 0, 2, 0, DateTimeKind.Utc)
        });
    }

    [Fact]
    public void EnumerateTo_WhenEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act, Assert
        Should.Throw<ArgumentException>(() => start.EnumerateTo(end, TimeSpan.FromMinutes(1)))
            .Message.ShouldBe("Start date must be before end date.");
    }

    [Fact]
    public void EnumerateMinutesTo_ReturnsCorrectSequence()
    {
        // Arrange
        var start = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2022, 1, 1, 0, 2, 0, DateTimeKind.Utc);

        // Act
        var result = start.EnumerateMinutesTo(end).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result.ShouldBeInOrder();
        result.ShouldBe(new[]
        {
            new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2022, 1, 1, 0, 1, 0, DateTimeKind.Utc),
            new DateTime(2022, 1, 1, 0, 2, 0, DateTimeKind.Utc)
        });
    }

    [Fact]
    public void EnumerateMinutesTo_WhenEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act, Assert
        Should.Throw<ArgumentException>(() => start.EnumerateMinutesTo(end))
            .Message.ShouldBe("Start date must be before end date.");
    }
}
