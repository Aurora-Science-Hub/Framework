using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="ReadOnlySpanExtensions"/>
/// </summary>
public sealed class ReadOnlySpanExtensionsTests
{
    [Theory]
    [InlineData("123", true)]
    [InlineData("123 ", true)]
    [InlineData(" 123", true)]
    [InlineData("", false)]
    [InlineData("abc", false)]
    public void IsParsableAsInt(string text, bool expected)
    {
        // Arrange
        var span = text.AsSpan();

        // Act
        var result = span.IsParsableAsInt();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("123 ", 123)]
    [InlineData(" 123", 123)]
    public void ParseIntInvariant_WhenParsable_ReturnsValue(string text, int expected)
    {
        // Arrange
        var span = text.AsSpan();

        // Act
        var result = span.ParseIntInvariant();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ParseIntInvariant_WhenNotParsable_ThrowsFormatException()
    {
        // Arrange, Act
        Action act = () => "abc".AsSpan().ParseIntInvariant();

        // Assert
        Should.Throw<FormatException>(act);
    }

    [Theory]
    [InlineData("123.45", 123.45f)]
    [InlineData("123.45 ", 123.45f)]
    [InlineData(" 123.45", 123.45f)]
    public void ParseFloatInvariant_WhenParsable_ReturnsValue(string text, float expected)
    {
        // Arrange
        var span = text.AsSpan();

        // Act
        var result = span.ParseFloatInvariant();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ParseFloatInvariant_WhenNotParsable_ThrowsFormatException()
    {
        // Arrange, Act
        Action act = () => "abc".AsSpan().ParseFloatInvariant();

        // Assert
        Should.Throw<FormatException>(act);
    }
}
