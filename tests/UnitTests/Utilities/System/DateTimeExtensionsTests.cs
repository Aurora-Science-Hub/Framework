using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="DateTimeExtensions"/>.
/// </summary>
public class DateTimeExtensionsTests
{
    [Fact(DisplayName = "TruncateToUtcMinute: clears seconds and milliseconds for UTC input")]
    public void TruncateToUtcMinute_ClearsSecondsAndMilliseconds()
    {
        // Arrange
        var value = new DateTime(2026, 8, 10, 18, 32, 2, 123, DateTimeKind.Utc);

        // Act
        var result = value.TruncateToUtcMinute();

        // Assert
        result.ShouldBe(new DateTime(2026, 8, 10, 18, 32, 0, DateTimeKind.Utc));
    }

    [Fact(DisplayName = "TruncateToUtcMinute: treats Unspecified as UTC and truncates")]
    public void TruncateToUtcMinute_TreatsUnspecifiedAsUtc()
    {
        // Arrange
        var value = new DateTime(2026, 8, 10, 18, 32, 45, 500, DateTimeKind.Unspecified);

        // Act
        var result = value.TruncateToUtcMinute();

        // Assert
        result.ShouldBe(new DateTime(2026, 8, 10, 18, 32, 0, DateTimeKind.Utc));
    }

    [Fact(DisplayName = "TruncateToUtcMinute: throws for Local DateTime")]
    public void TruncateToUtcMinute_WhenLocal_ThrowsArgumentException()
    {
        // Arrange
        var value = new DateTime(2026, 8, 10, 21, 32, 2, 123, DateTimeKind.Local);

        // Act / Assert
        var exception = Should.Throw<ArgumentException>(() => value.TruncateToUtcMinute());
        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldContain("UTC or Unspecified");
    }

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
