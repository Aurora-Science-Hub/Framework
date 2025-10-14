using AuroraScienceHub.Framework.Http;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Http;

/// <summary>
/// Unit tests for <see cref="UrlBuilder"/>.
/// </summary>
public sealed class UrlBuilderTest
{
    [Fact]
    public void Build_FromString_ReturnsUrl()
    {
        // Arrange
        var url = "http://example.com";

        // Act
        var result = UrlBuilder
            .From(url)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe(url);
    }

    [Fact]
    public void Build_FromUri_ReturnsUrl()
    {
        // Arrange
        var url = new Uri("http://example.com");

        // Act
        var result = UrlBuilder
            .From(url)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe(url.OriginalString);
    }

    [Fact]
    public void Build_FromBaseAndRelativeUrl_ReturnsUrl()
    {
        // Arrange
        var baseUrl = new Uri("http://example.com");
        var relativeUrl = "/api";

        // Act
        var result = UrlBuilder
            .From(baseUrl, relativeUrl)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe("http://example.com/api");
    }

    [Fact]
    public void Build_WithParameter_ReturnsUrl()
    {
        // Arrange
        var url = "http://example.com";
        var name = "name";
        var value = "value";

        // Act
        var result = UrlBuilder
            .From(url)
            .AddParameter(name, value)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe("http://example.com?name=value");
    }

    [Fact]
    public void Build_WithSegment_ReturnsUrl()
    {
        // Arrange
        var url = "http://example.com/{segment}";
        var segmentName = "segment";
        var value = "value";

        // Act
        var result = UrlBuilder
            .From(url)
            .ReplaceSegment(segmentName, value)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe("http://example.com/value");
    }

    [Fact]
    public void Build_WhenCalledTwice_ThrowsException()
    {
        // Arrange
        var url = "http://example.com";

        // Act
        var builder = UrlBuilder.From(url);
        builder.Build();

        // Assert
        Should.Throw<InvalidOperationException>(() => builder.Build())
            .Message.ShouldBe("The URL has already been built.");
    }

    [Fact]
    public void Build_WhenNullParameter_SkipsParameter()
    {
        // Arrange
        var url = "http://example.com";
        var name = "name";
        string? value = null;

        // Act
        var result = UrlBuilder
            .From(url)
            .AddParameter(name, value)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe("http://example.com");
    }

    [Fact]
    public void Build_WhenEmptyParameter_SkipsParameter()
    {
        // Arrange
        var url = "http://example.com";
        var name = "name";
        var value = string.Empty;

        // Act
        var result = UrlBuilder
            .From(url)
            .AddParameter(name, value)
            .Build();

        // Assert
        result.ShouldNotBeNull();
        result.OriginalString.ShouldBe("http://example.com");
    }

    [Fact]
    public void Build_WhenUnreplacedSegment_ThrowsException()
    {
        // Arrange
        var url = "http://example.com/{segment}";

        // Act
        var builder = UrlBuilder.From(url);

        // Assert
        Should.Throw<InvalidOperationException>(() => builder.Build())
            .Message.ShouldBe("There are unreplaced placeholders in the URL");
    }

    [Fact]
    public void Build_WhenParameterExists_ThrowsException()
    {
        // Arrange
        var url = "http://example.com";
        var name = "name";
        var value = "value";

        // Act
        var builder = UrlBuilder
            .From(url)
            .AddParameter(name, value);

        // Assert
        Should.Throw<ArgumentException>(() => builder.AddParameter(name, value))
            .Message.ShouldBe("Parameter with name 'name' already exists (Parameter 'name')");
    }

    [Fact]
    public void Build_WhenParameterNameIsEmpty_ThrowsException()
    {
        // Arrange
        var url = "http://example.com";
        var name = string.Empty;
        var value = "value";

        // Act
        var builder = UrlBuilder.From(url);

        // Assert
        Should.Throw<ArgumentException>(() => builder.AddParameter(name, value))
            .Message.ShouldBe("Parameter name cannot be null or empty (Parameter 'name')");
    }
}
