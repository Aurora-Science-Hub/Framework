using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

public sealed partial class BlobIdTests
{
    [Fact(DisplayName = "Equals: Returns true when comparing with same instance")]
    public void Equals_WithSameBlobId_ReturnsTrue()
    {
        // Arrange
        var blobId = BlobId.New("test-bucket");

        // Act & Assert
        blobId.Equals(blobId).ShouldBeTrue();
    }

    [Fact(DisplayName = "Equals: Returns false when comparing with null")]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var blobId = BlobId.New("test-bucket");

        // Act & Assert
        blobId.Equals(null).ShouldBeFalse();
    }

    [Fact(DisplayName = "Equals: Returns false when comparing different BlobIds")]
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

    [Fact(DisplayName = "Equals: Returns true when comparing with parsed same BlobId")]
    public void Equals_WithParsedSameBlobId_ReturnsTrue()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var parsed = BlobId.Parse(original.Value);

        // Act & Assert
        original.Equals(parsed).ShouldBeTrue();
        (original == parsed).ShouldBeTrue();
    }

    [Fact(DisplayName = "GetHashCode: Returns same hash code for equal BlobIds")]
    public void GetHashCode_ForEqualBlobIds_ReturnsSameHashCode()
    {
        // Arrange
        var original = BlobId.New("test-bucket");
        var parsed = BlobId.Parse(original.Value);

        // Act & Assert
        original.GetHashCode().ShouldBe(parsed.GetHashCode());
    }

    [Fact(DisplayName = "ToString: Returns value property")]
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
}

