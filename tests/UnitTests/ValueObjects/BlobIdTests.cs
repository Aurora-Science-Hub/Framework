using AuroraScienceHub.Framework.ValueObjects.Blobls;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

/// <summary>
/// Tests for <see cref="BlobId"/>
/// </summary>
public sealed partial class BlobIdTests
{
    [Fact(DisplayName = "New: Creates BlobId with valid bucket name")]
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

    [Theory(DisplayName = "New: Creates BlobIds with various valid bucket names")]
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

    [Theory(DisplayName = "New: Throws ArgumentException for invalid bucket names")]
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

    [Fact(DisplayName = "New: Generates unique ObjectIds for same bucket")]
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
}

