using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="FloatExtensions"/>.
/// </summary>
public class FloatExtensionsTests
{
    [Theory]
    [InlineData(1.0f, 1.0f, 0.00001f)]
    [InlineData(1.0f, 1.000001f, 0.00001f)]
    [InlineData(1.0f, 1.000009f, 0.00001f)]
    public void IsCloseTo_WhenWithinEpsilon_ReturnsTrue(float a, float b, float epsilon)
    {
        // Act
        var result = a.IsCloseTo(b, epsilon);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsCloseTo_WhenNotWithinEpsilon_ReturnsFalse()
    {
        // Arrange
        var a = 1.0f;
        var b = 1.00001f;
        var epsilon = 0.000001f;

        // Act
        var result = a.IsCloseTo(b, epsilon);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsCloseTo_WhenBothValuesAreNull_ReturnsTrue()
    {
        // Arrange
        float? a = null;
        float? b = null;

        // Act
        var result = a.IsCloseTo(b, 0.00001f);

        // Assert
        result.ShouldBeTrue();
    }
}
