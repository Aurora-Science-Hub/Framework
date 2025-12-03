using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

public sealed partial class BlobIdTests
{
    [Fact(DisplayName = "TryParse (String): Returns true and BlobId for valid string")]
    public void TryParse_WithValidString_ReturnsTrueAndBlobId()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var text = original.Value;

        // Act
        var result = BlobId.TryParse(text, out var parsed);

        // Assert
        result.ShouldBeTrue();
        parsed.ShouldNotBeNull();
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
    }

    [Theory(DisplayName = "TryParse (String): Returns false for invalid strings")]
    [InlineData("")]
    [InlineData("test-bucket_abc123")]
    [InlineData("blb_test-bucket")]
    [InlineData("blb__abc123")]
    [InlineData("invalid")]
    public void TryParse_WithInvalidString_ReturnsFalse(string text)
    {
        // Act
        var result = BlobId.TryParse(text, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    [Fact(DisplayName = "TryParse (String): Returns false for null string")]
    public void TryParse_WithNullString_ReturnsFalse()
    {
        // Act
        var result = BlobId.TryParse(null, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    [Fact(DisplayName = "TryParse (Span): Returns true and BlobId for valid span")]
    public void TryParse_WithValidSpan_ReturnsTrueAndBlobId()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var text = original.Value.AsSpan();

        // Act
        var result = BlobId.TryParse(text, null, out var parsed);

        // Assert
        result.ShouldBeTrue();
        parsed.ShouldNotBeNull();
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
    }

    [Fact(DisplayName = "TryParse (Span): Returns false for empty span")]
    public void TryParse_WithEmptySpan_ReturnsFalse()
    {
        // Arrange
        var text = ReadOnlySpan<char>.Empty;

        // Act
        var result = BlobId.TryParse(text, null, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    [Theory(DisplayName = "TryParse (Span): Returns false for invalid spans")]
    [InlineData("test-bucket_abc123")]
    [InlineData("blb_test-bucket")]
    [InlineData("blb__abc123")]
    [InlineData("blb_test-bucket_")]
    [InlineData("invalid")]
    public void TryParse_WithInvalidSpan_ReturnsFalse(string text)
    {
        // Arrange
        var span = text.AsSpan();

        // Act
        var result = BlobId.TryParse(span, null, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }
}

