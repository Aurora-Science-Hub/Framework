using AuroraScienceHub.Framework.ValueObjects.Blobls;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

/// <summary>
/// Tests for <see cref="BlobId"/>
/// </summary>
public sealed class BlobIdTests
{
    #region New Tests

    [Fact]
    public void New_WithValidBucketName_CreatesBlobId()
    {
        // Arrange
        var bucketName = "test-bucket";

        // Act
        var blobId = BlobId.New(bucketName);

        // Assert
        blobId.ShouldNotBeNull();
        blobId.BucketName.ShouldBe(bucketName);
        blobId.ObjectId.ShouldNotBeNullOrEmpty();
        blobId.Value.ShouldStartWith($"blb_{bucketName}_");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("test123")]
    [InlineData("my.bucket")]
    [InlineData("test.bucket.123")]
    [InlineData("test-bucket")]
    [InlineData("my-test-bucket")]
    public void New_WithValidBucketNames_CreatesBlobIds(string bucketName)
    {
        // Act
        var blobId = BlobId.New(bucketName);

        // Assert
        blobId.ShouldNotBeNull();
        blobId.BucketName.ShouldBe(bucketName);
        blobId.ObjectId.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("test_bucket")] // Contains delimiter
    [InlineData("ab")] // Too short
    [InlineData("TestBucket")] // Uppercase letters
    [InlineData(".testbucket")] // Starts with dot
    [InlineData("testbucket.")] // Ends with dot
    [InlineData("-testbucket")] // Starts with hyphen
    [InlineData("testbucket-")] // Ends with hyphen
    [InlineData("")] // Empty
    public void New_WithInvalidBucketName_ThrowsArgumentException(string bucketName)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => BlobId.New(bucketName));
    }

    [Fact]
    public void New_GeneratesUniqueObjectIds()
    {
        // Arrange
        var bucketName = "test-bucket";

        // Act
        var blobId1 = BlobId.New(bucketName);
        var blobId2 = BlobId.New(bucketName);

        // Assert
        blobId1.ObjectId.ShouldNotBe(blobId2.ObjectId);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var bucketName = "test-bucket";
        var blobId = BlobId.New(bucketName);

        // Act
        var result = blobId.ToString();

        // Assert
        result.ShouldBe(blobId.Value);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameBlobId_ReturnsTrue()
    {
        // Arrange
        var blobId = BlobId.New("test-bucket");

        // Act & Assert
        blobId.Equals(blobId).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var blobId = BlobId.New("test-bucket");

        // Act & Assert
        blobId.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WithDifferentBlobId_ReturnsFalse()
    {
        // Arrange
        var blobId1 = BlobId.New("test-bucket");
        var blobId2 = BlobId.New("test-bucket");

        // Act & Assert
        blobId1.Equals(blobId2).ShouldBeFalse();
        (blobId1 == blobId2).ShouldBeFalse();
        (blobId1 != blobId2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithParsedSameBlobId_ReturnsTrue()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var parsed = BlobId.Parse(original.Value);

        // Act & Assert
        original.Equals(parsed).ShouldBeTrue();
        (original == parsed).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_ForEqualBlobIds_ReturnsSameHashCode()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var parsed = BlobId.Parse(original.Value);

        // Act & Assert
        original.GetHashCode().ShouldBe(parsed.GetHashCode());
    }

    #endregion

    #region Parse Tests

    [Fact]
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

    [Theory]
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

    [Theory]
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

    [Fact]
    public void Parse_WithNullString_ThrowsFormatException()
    {
        // Act & Assert
        Should.Throw<FormatException>(() => BlobId.Parse(null));
    }

    #endregion

    #region TryParse String Tests

    [Fact]
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

    [Theory]
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

    [Fact]
    public void TryParse_WithNullString_ReturnsFalse()
    {
        // Act
        var result = BlobId.TryParse(null, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    #endregion

    #region TryParse Span Tests

    [Fact]
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

    [Fact]
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

    [Theory]
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

    [Fact]
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

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("blb_test-bucket")]
    public void Parse_Span_WithInvalidSpan_ThrowsFormatException(string text)
    {
        // Act & Assert
        Should.Throw<FormatException>(() => BlobId.Parse(text.AsSpan(), null));
    }

    #endregion

    #region Round-trip Tests

    [Fact]
    public void RoundTrip_CreateParseToString_PreservesValue()
    {
        // Arrange
        var bucketName = "test-bucket";
        var original = BlobId.New(bucketName);

        // Act
        var parsed = BlobId.Parse(original.ToString());

        // Assert
        parsed.Value.ShouldBe(original.Value);
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
    }

    [Fact]
    public void RoundTrip_ParseToStringParse_PreservesValue()
    {
        // Arrange
        var text = "blb_test-bucket_abc123xyz";
        var firstParse = BlobId.Parse(text);

        // Act
        var secondParse = BlobId.Parse(firstParse.ToString());

        // Assert
        secondParse.Value.ShouldBe(firstParse.Value);
        secondParse.BucketName.ShouldBe(firstParse.BucketName);
        secondParse.ObjectId.ShouldBe(firstParse.ObjectId);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Parse_WithMinimumValidBucketName_Works()
    {
        // Arrange - minimum valid bucket name is 3 characters
        var text = "blb_abc_xyz";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("abc");
        parsed.ObjectId.ShouldBe("xyz");
    }

    [Fact]
    public void Parse_WithMaximumValidBucketName_Works()
    {
        // Arrange - maximum valid bucket name is 63 characters
        var bucketName = new string('a', 63);
        var text = $"blb_{bucketName}_xyz";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe(bucketName);
        parsed.ObjectId.ShouldBe("xyz");
    }

    [Fact]
    public void Parse_WithBucketNameContainingDots_Works()
    {
        // Arrange
        var text = "blb_my.test.bucket_xyz123";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("my.test.bucket");
        parsed.ObjectId.ShouldBe("xyz123");
    }

    [Fact]
    public void Parse_WithBucketNameContainingHyphens_Works()
    {
        // Arrange
        var text = "blb_my-test-bucket_xyz123";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("my-test-bucket");
        parsed.ObjectId.ShouldBe("xyz123");
    }

    [Fact]
    public void Parse_WithBucketNameContainingDotsAndHyphens_Works()
    {
        // Arrange
        var text = "blb_my-test.bucket-2024_xyz123";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("my-test.bucket-2024");
        parsed.ObjectId.ShouldBe("xyz123");
    }

    [Fact]
    public void Parse_WithLongObjectId_Works()
    {
        // Arrange
        var longObjectId = new string('x', 100);
        var text = $"blb_test-bucket_{longObjectId}";

        // Act
        var parsed = BlobId.Parse(text);

        // Assert
        parsed.BucketName.ShouldBe("test-bucket");
        parsed.ObjectId.ShouldBe(longObjectId);
    }

    [Fact]
    public void TryParse_WithBucketNameTooLong_ReturnsFalse()
    {
        // Arrange - bucket name is 64 characters (too long)
        var bucketName = new string('a', 64);
        var text = $"blb_{bucketName}_xyz";

        // Act
        var result = BlobId.TryParse(text, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    [Fact]
    public void TryParse_WithBucketNameTooShort_ReturnsFalse()
    {
        // Arrange - bucket name is 2 characters (too short)
        var text = "blb_ab_xyz";

        // Act
        var result = BlobId.TryParse(text, out var parsed);

        // Assert
        result.ShouldBeFalse();
        parsed.ShouldBeNull();
    }

    #endregion
}

