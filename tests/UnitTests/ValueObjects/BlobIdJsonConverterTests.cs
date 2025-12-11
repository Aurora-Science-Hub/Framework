using System.Text.Json;
using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

/// <summary>
/// Tests for <see cref="BlobIdJsonConverter"/>
/// </summary>
public sealed class BlobIdJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public BlobIdJsonConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new BlobIdJsonConverter());
    }

    [Fact(DisplayName = "Read deserializes BlobId from JSON string")]
    public void Read_DeserializesBlobIdFromJsonString()
    {
        // Arrange
        var json = "\"blb_test-bucket_photos/abc123.jpg\"";

        // Act
        var result = JsonSerializer.Deserialize<BlobId>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.BucketName.ShouldBe("test-bucket");
        result.ObjectKey.ShouldBe("photos/abc123.jpg");
        result.Value.ShouldBe("blb_test-bucket_photos/abc123.jpg");
    }

    [Fact(DisplayName = "Write serializes BlobId to JSON string")]
    public void Write_SerializesBlobIdToJsonString()
    {
        // Arrange
        var blobId = BlobId.Parse("blb_test-bucket_abc123");

        // Act
        var json = JsonSerializer.Serialize(blobId, _options);

        // Assert
        json.ShouldBe("\"blb_test-bucket_abc123\"");
    }

    [Fact(DisplayName = "WriteAsPropertyName writes BlobId as property name")]
    public void WriteAsPropertyName_WritesBlobIdAsPropertyName()
    {
        // Arrange
        var blobId = BlobId.Parse("blb_test-bucket_abc123");
        var dictionary = new Dictionary<BlobId, string>
        {
            { blobId, "value" }
        };

        // Act
        var json = JsonSerializer.Serialize(dictionary, _options);

        // Assert
        json.ShouldBe("{\"blb_test-bucket_abc123\":\"value\"}");
    }

    [Fact(DisplayName = "ReadAsPropertyName reads BlobId from property name")]
    public void ReadAsPropertyName_ReadsBlobIdFromPropertyName()
    {
        // Arrange
        var json = "{\"blb_test-bucket_abc123\":\"value\"}";

        // Act
        var result = JsonSerializer.Deserialize<Dictionary<BlobId, string>>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        var key = result.Keys.First();
        key.Value.ShouldBe("blb_test-bucket_abc123");
        result[key].ShouldBe("value");
    }

    [Fact(DisplayName = "Roundtrip serialization preserves BlobId")]
    public void Roundtrip_PreservesBlobId()
    {
        // Arrange
        var original = BlobId.New("my-bucket", "photos", "jpg");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<BlobId>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.ShouldBe(original);
        deserialized.BucketName.ShouldBe(original.BucketName);
        deserialized.ObjectKey.ShouldBe(original.ObjectKey);
    }
}
