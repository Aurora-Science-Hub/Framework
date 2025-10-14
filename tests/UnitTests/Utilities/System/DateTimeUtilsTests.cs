using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Unit tests for <see cref="DateTimeUtils"/>.
/// </summary>
public sealed class DateTimeUtilsTests
{
    [Fact]
    public void Min_WhenBothNull_ReturnsNull()
    {
        // Act
        var result = DateTimeUtils.Min(null, null);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Min_WhenFirstNull_ReturnsSecond()
    {
        // Arrange
        var value = new DateTime(2021, 1, 1);

        // Act
        var result = DateTimeUtils.Min(null, value);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void Min_WhenSecondNull_ReturnsFirst()
    {
        // Arrange
        var value = new DateTime(2021, 1, 1);

        // Act
        var result = DateTimeUtils.Min(value, null);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void Min_WhenFirstLessThanSecond_ReturnsFirst()
    {
        // Arrange
        var value1 = new DateTime(2021, 1, 1);
        var value2 = new DateTime(2021, 1, 2);

        // Act
        var result = DateTimeUtils.Min(value1, value2);

        // Assert
        result.ShouldBe(value1);
    }

    [Fact]
    public void Max_WhenBothNull_ReturnsNull()
    {
        // Act
        var result = DateTimeUtils.Max(null, null);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Max_WhenFirstNull_ReturnsSecond()
    {
        // Arrange
        var value = new DateTime(2021, 1, 1);

        // Act
        var result = DateTimeUtils.Max(null, value);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void Max_WhenSecondNull_ReturnsFirst()
    {
        // Arrange
        var value = new DateTime(2021, 1, 1);

        // Act
        var result = DateTimeUtils.Max(value, null);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void Max_WhenFirstGreaterThanSecond_ReturnsFirst()
    {
        // Arrange
        var value1 = new DateTime(2021, 1, 2);
        var value2 = new DateTime(2021, 1, 1);

        // Act
        var result = DateTimeUtils.Max(value1, value2);

        // Assert
        result.ShouldBe(value1);
    }
}
