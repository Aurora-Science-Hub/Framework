using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="StringExtensions"/>
/// </summary>
public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("SpaceWeather", "space-weather")]
    [InlineData("SpaceWeatherUI", "space-weather-ui")]
    [InlineData("", "")]
    public void ShouldConvertPascalToKebabCase(string pascalCaseText, string expectedKebabCaseText)
    {
        // Act
        var realKebabCasteText = pascalCaseText.PascalToKebabCase();

        // Assert
        realKebabCasteText.ShouldBe(expectedKebabCaseText);
    }

    [Theory]
    [InlineData("SpaceWeather", "space_weather")]
    [InlineData("PreliminaryDataImport", "preliminary_data_import")]
    [InlineData("", "")]
    public void ShouldConvertPascalToLowerSnakeCase(string pascalCaseText, string expectedSnakeCaseText)
    {
        // Act
        var realSnakeCaseText = pascalCaseText.PascalToLowerSnakeCase();

        // Assert
        realSnakeCaseText.ShouldBe(expectedSnakeCaseText);
    }

    [Fact]
    public void SplitLines_WhenNotEmptyString_ReturnsLines()
    {
        // Arrange, Act
        var text = "line1\nline2\r\nline3\rline4";
        var result = text.SplitLines();

        // Assert
        var expectedResult = new[] { "line1", "line2", "line3", "line4" };
        var index = 0;
        while (result.MoveNext())
        {
            var entry = result.Current;
            entry.Line.ToString().ShouldBe(expectedResult[index]);
            index++;
        }
    }

    [Fact]
    public void SplitLines_WhenEmptyString_ReturnsNothing()
    {
        // Arrange, Act
        var text = string.Empty;
        var result = text.SplitLines();

        // Assert
        result.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void SplitLines_WhenNothingToSplit_ReturnsSameString()
    {
        // Arrange, Act
        var text = "line1";
        var result = text.SplitLines();

        // Assert
        result.MoveNext().ShouldBeTrue();
        result.Current.Line.ToString().ShouldBe(text);
    }

    [Fact]
    public void Split_ReturnsChunks()
    {
        // Arrange, Act
        var text = "1234567890";

        // Assert
        var result = text.Split(3).ToArray();
        result.ShouldBe(["123", "456", "789", "0"]);
    }
}
