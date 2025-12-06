using AuroraScienceHub.Framework.ValueObjects.Blobs;
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
        blobId.ObjectKey.ShouldNotBeNullOrEmpty();
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
        blobId.ObjectKey.ShouldNotBeNullOrEmpty();
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

    [Fact(DisplayName = "New: Generates unique ObjectKeys for same bucket")]
    public void New_GeneratesUniqueObjectKeys()
    {
        // Arrange
        var bucketName = "test-bucket";

        // Act
        var blobId1 = BlobId.New(bucketName);
        var blobId2 = BlobId.New(bucketName);

        // Assert
        blobId1.ObjectKey.ShouldNotBe(blobId2.ObjectKey);
    }

    [Fact(DisplayName = "New: Creates BlobId with extension")]
    public void New_WithExtension_CreatesBlobIdWithExtension()
    {
        // Arrange
        var bucketName = "test-bucket";
        var extension = "pdf";

        // Act
        var blobId = BlobId.New(bucketName, extension);

        // Assert
        blobId.ShouldNotBeNull();
        blobId.BucketName.ShouldBe(bucketName);
        blobId.ObjectKey.ShouldEndWith($".{extension}");
        blobId.Value.ShouldContain($".{extension}");
    }

    [Theory(DisplayName = "New: Normalizes extension correctly")]
    [InlineData(".pdf", ".pdf")]
    [InlineData("PDF", ".pdf")]
    [InlineData(".JPG", ".jpg")]
    [InlineData("png", ".png")]
    public void New_WithExtension_NormalizesExtension(string input, string expectedSuffix)
    {
        // Act
        var blobId = BlobId.New("test-bucket", input);

        // Assert
        blobId.ObjectKey.ShouldEndWith(expectedSuffix);
    }

    [Fact(DisplayName = "New: Creates BlobId with prefix and extension")]
    public void New_WithPrefixAndExtension_CreatesBlobId()
    {
        // Arrange
        var bucketName = "uploads";
        var prefix = "users/2024/photos";
        var extension = "jpg";

        // Act
        var blobId = BlobId.New(bucketName, prefix, extension);

        // Assert
        blobId.ShouldNotBeNull();
        blobId.BucketName.ShouldBe(bucketName);
        blobId.ObjectKey.ShouldStartWith($"{prefix}/");
        blobId.ObjectKey.ShouldEndWith($".{extension}");
    }

    [Theory(DisplayName = "New: Normalizes prefix correctly")]
    [InlineData("/users/photos/", "users/photos/")]
    [InlineData("  users/photos  ", "users/photos/")]
    [InlineData("users", "users/")]
    public void New_WithPrefix_NormalizesPrefix(string input, string expectedStart)
    {
        // Act
        var blobId = BlobId.New("test-bucket", input, null);

        // Assert
        blobId.ObjectKey.ShouldStartWith(expectedStart);
    }

    [Theory(DisplayName = "New: Throws ArgumentException for invalid extension")]
    [InlineData("pdf_doc")] // Contains delimiter
    [InlineData("pdf/doc")] // Contains path separator
    public void New_WithInvalidExtension_ThrowsArgumentException(string extension)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => BlobId.New("test-bucket", extension));
    }

    [Theory(DisplayName = "New: Throws ArgumentException for invalid prefix")]
    [InlineData("users_photos")] // Contains delimiter
    public void New_WithInvalidPrefix_ThrowsArgumentException(string prefix)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => BlobId.New("test-bucket", prefix, "jpg"));
    }

    [Fact(DisplayName = "New: Creates BlobId with null extension and prefix")]
    public void New_WithNullExtensionAndPrefix_CreatesBlobId()
    {
        // Act
        var blobId = BlobId.New("test-bucket", null, null);

        // Assert
        blobId.ObjectKey.ShouldNotBeNullOrEmpty();
        blobId.ObjectKey.ShouldNotContain("/");
        blobId.ObjectKey.ShouldNotContain(".");
    }

    [Fact(DisplayName = "ObjectKey: Returns correct format with prefix and extension")]
    public void ObjectKey_WithPrefixAndExtension_ReturnsCorrectFormat()
    {
        // Arrange
        var blobId = BlobId.New("bucket", "folder/subfolder", "txt");

        // Act
        var objectKey = blobId.ObjectKey;

        // Assert
        objectKey.ShouldStartWith("folder/subfolder/");
        objectKey.ShouldEndWith(".txt");
    }

    [Fact(DisplayName = "FileName: Returns file name with extension when prefix exists")]
    public void FileName_WithPrefix_ReturnsFileName()
    {
        // Arrange
        var blobId = BlobId.New("bucket", "users/photos", "jpg");

        // Act
        var fileName = blobId.FileName;

        // Assert
        fileName.ShouldNotContain("/");
        fileName.ShouldEndWith(".jpg");
    }

    [Fact(DisplayName = "FileName: Returns ObjectKey when no prefix")]
    public void FileName_WithoutPrefix_ReturnsObjectKey()
    {
        // Arrange
        var blobId = BlobId.New("bucket", "pdf");

        // Act
        var fileName = blobId.FileName;

        // Assert
        fileName.ShouldBe(blobId.ObjectKey);
        fileName.ShouldEndWith(".pdf");
    }

    [Fact(DisplayName = "FileName: Returns ObjectKey when no prefix and no extension")]
    public void FileName_WithoutPrefixAndExtension_ReturnsObjectKey()
    {
        // Arrange
        var blobId = BlobId.New("bucket");

        // Act
        var fileName = blobId.FileName;

        // Assert
        fileName.ShouldBe(blobId.ObjectKey);
        fileName.ShouldNotContain("/");
        fileName.ShouldNotContain(".");
    }

    [Fact(DisplayName = "FileName: Parsed BlobId extracts FileName correctly")]
    public void FileName_ParsedBlobId_ExtractsCorrectly()
    {
        // Arrange
        var text = "blb_bucket_users/2024/photos/abc123.jpg";

        // Act
        var blobId = BlobId.Parse(text);

        // Assert
        blobId.FileName.ShouldBe("abc123.jpg");
    }
}

