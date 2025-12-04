using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

public sealed partial class BlobIdTests
{
    [Fact(DisplayName = "Round-trip: Create → Parse → ToString preserves value")]
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

    [Fact(DisplayName = "Round-trip: Create with extension → Parse preserves extension")]
    public void RoundTrip_WithExtension_PreservesExtension()
    {
        // Arrange
        var original = BlobId.New("test-bucket", "pdf");

        // Act
        var parsed = BlobId.Parse(original.ToString());

        // Assert
        parsed.Value.ShouldBe(original.Value);
        parsed.Extension.ShouldBe(original.Extension);
        parsed.ObjectId.ShouldBe(original.ObjectId);
    }

    [Fact(DisplayName = "Round-trip: Create with prefix and extension → Parse preserves all")]
    public void RoundTrip_WithPrefixAndExtension_PreservesAll()
    {
        // Arrange
        var original = BlobId.New("uploads", "users/photos", "jpg");

        // Act
        var parsed = BlobId.Parse(original.ToString());

        // Assert
        parsed.Value.ShouldBe(original.Value);
        parsed.BucketName.ShouldBe(original.BucketName);
        parsed.ObjectId.ShouldBe(original.ObjectId);
        parsed.NamePrefix.ShouldBe(original.NamePrefix);
        parsed.Extension.ShouldBe(original.Extension);
        parsed.ObjectKey.ShouldBe(original.ObjectKey);
    }

    [Fact(DisplayName = "Round-trip: Parse → ToString → Parse preserves value")]
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

    [Fact(DisplayName = "Edge case: Parses minimum valid bucket name (3 chars)")]
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

    [Fact(DisplayName = "Edge case: Parses maximum valid bucket name (63 chars)")]
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

    [Fact(DisplayName = "Edge case: Parses bucket name containing dots")]
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

    [Fact(DisplayName = "Edge case: Parses bucket name containing hyphens")]
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

    [Fact(DisplayName = "Edge case: Parses bucket name with dots and hyphens")]
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

    [Fact(DisplayName = "Edge case: Parses long ObjectId (100 chars)")]
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

    [Fact(DisplayName = "Edge case: Rejects bucket name too long (64 chars)")]
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

    [Fact(DisplayName = "Edge case: Rejects bucket name too short (2 chars)")]
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
}

