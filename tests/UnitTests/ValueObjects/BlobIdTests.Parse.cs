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
    [InlineData("blb_bucket_abc123.pdf")]
    [InlineData("blb_bucket_folder/abc123")]
    [InlineData("blb_bucket_folder/subfolder/abc123.jpg")]
    public void Parse_WithValidFormats_ReturnsBlobId(string text)
    {
        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.ShouldNotBeNull();
        parsed.Value.ShouldBe(text);
    }

    [Fact(DisplayName = "Parse: Correctly parses BlobId with extension")]
    public void Parse_WithExtension_ParsesCorrectly()
    {
        // Arrange
        var text = "blb_test-bucket_abc123.pdf";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("test-bucket");
        parsed.ObjectId.ShouldBe("abc123");
        parsed.Extension.ShouldBe("pdf");
        parsed.NamePrefix.ShouldBeNull();
    }

    [Fact(DisplayName = "Parse: Correctly parses BlobId with prefix")]
    public void Parse_WithPrefix_ParsesCorrectly()
    {
        // Arrange
        var text = "blb_test-bucket_users/photos/abc123";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("test-bucket");
        parsed.ObjectId.ShouldBe("abc123");
        parsed.NamePrefix.ShouldBe("users/photos");
        parsed.Extension.ShouldBeNull();
    }

    [Fact(DisplayName = "Parse: Correctly parses BlobId with prefix and extension")]
    public void Parse_WithPrefixAndExtension_ParsesCorrectly()
    {
        // Arrange
        var text = "blb_uploads_users/2024/photos/abc123.jpg";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("uploads");
        parsed.ObjectId.ShouldBe("abc123");
        parsed.NamePrefix.ShouldBe("users/2024/photos");
        parsed.Extension.ShouldBe("jpg");
        parsed.ObjectKey.ShouldBe("users/2024/photos/abc123.jpg");
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

