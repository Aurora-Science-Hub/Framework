using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

public sealed partial class BlobIdTests
{
    [Fact(DisplayName = "Parse: Successfully parses valid BlobId string")]
    public void Parse_WithValidString_ReturnsBlobId()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var text = original.Value;

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
        parsed.Value.ShouldBe(original.Value);
    }

    [Theory(DisplayName = "Parse: Successfully parses various valid formats")]
    [InlineData("blb_test-bucket_abc123")]
    [InlineData("blb_my.bucket_xyz789")]
    [InlineData("blb_abc_test")]
    [InlineData("blb_my-test-bucket_id123")]
    public void Parse_WithValidFormats_ReturnsBlobId(string text)
    {
        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.Value.ShouldBe(text);
    }

    [Theory(DisplayName = "Parse: Throws FormatException for invalid strings")]
    [InlineData("")] // Empty string
    [InlineData("test-bucket_abc123")] // Missing prefix
    [InlineData("blb_test-bucket")] // Missing object ID
    [InlineData("blb__abc123")] // Empty bucket name
    [InlineData("blb_test-bucket_")] // Empty object ID
    [InlineData("invalid_format")] // Wrong format
    [InlineData("blb_")] // Only prefix
    [InlineData("blb_TestBucket_abc")] // Invalid bucket name (uppercase)
    [InlineData("blb_-testbucket_abc")] // Invalid bucket name (starts with hyphen)
    [InlineData("blb_.testbucket_abc")] // Invalid bucket name (starts with dot)
    public void Parse_WithInvalidString_ThrowsFormatException(string text)
    {
        // Act & Assert
        Should.Throw<FormatException>(() => BlobId.Parse(text));
    }

    [Fact(DisplayName = "Parse: Throws FormatException for null string")]
    public void Parse_WithNullString_ThrowsFormatException()
    {
        // Act & Assert
        Should.Throw<FormatException>(() => BlobId.Parse(null));
    }

    [Fact(DisplayName = "Parse (Span): Successfully parses valid span")]
    public void Parse_Span_WithValidSpan_ReturnsBlobId()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var text = original.Value.AsSpan();

        // Act
        var parsed = BlobId.Parse(text, null);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
    }

    [Theory(DisplayName = "Parse (Span): Throws FormatException for invalid spans")]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("blb_test-bucket")]
    public void Parse_Span_WithInvalidSpan_ThrowsFormatException(string text)
    {
        // Act & Assert
        Should.Throw<FormatException>(() => BlobId.Parse(text.AsSpan(), null));
    }
}

