using System.Text.Json;
using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

/// <summary>
/// Tests for <see cref="BlobIdJsonConverterFactory"/>
/// </summary>
public sealed class BlobIdJsonConverterFactoryTests
{
    private readonly BlobIdJsonConverterFactory _factory;

    public BlobIdJsonConverterFactoryTests()
    {
        _factory = new BlobIdJsonConverterFactory();
    }

    [Fact(DisplayName = "CanConvert returns true for BlobId type")]
    public void CanConvert_WhenBlobIdType_ReturnsTrue()
    {
        // Arrange
        var type = typeof(BlobId);

        // Act
        var result = _factory.CanConvert(type);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "CanConvert returns false for non-BlobId types")]
    public void CanConvert_WhenNonBlobIdType_ReturnsFalse()
    {
        // Arrange & Act & Assert
        _factory.CanConvert(typeof(string)).ShouldBeFalse();
        _factory.CanConvert(typeof(int)).ShouldBeFalse();
        _factory.CanConvert(typeof(Guid)).ShouldBeFalse();
    }

    [Fact(DisplayName = "CreateConverter returns BlobIdJsonConverter")]
    public void CreateConverter_ReturnsBlobIdJsonConverter()
    {
        // Arrange
        var options = new JsonSerializerOptions();

        // Act
        var converter = _factory.CreateConverter(typeof(BlobId), options);

        // Assert
        converter.ShouldNotBeNull();
        converter.ShouldBeOfType<BlobIdJsonConverter>();
    }

    [Fact(DisplayName = "CreateConverter returns same instance on multiple calls")]
    public void CreateConverter_ReturnsSameInstanceOnMultipleCalls()
    {
        // Arrange
        var options = new JsonSerializerOptions();

        // Act
        var converter1 = _factory.CreateConverter(typeof(BlobId), options);
        var converter2 = _factory.CreateConverter(typeof(BlobId), options);

        // Assert
        converter1.ShouldBeSameAs(converter2);
    }

    [Fact(DisplayName = "Factory serializes and deserializes BlobId correctly")]
    public void Factory_SerializesAndDeserializesBlobId()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(_factory);
        var blobId = BlobId.Parse("blb_test-bucket_abc123");

        // Act
        var json = JsonSerializer.Serialize(blobId, options);
        var result = JsonSerializer.Deserialize<BlobId>(json, options);

        // Assert
        json.ShouldBe("\"blb_test-bucket_abc123\"");
        result.ShouldNotBeNull();
        result.Value.ShouldBe("blb_test-bucket_abc123");
    }

    [Fact(DisplayName = "Factory works with complex objects containing BlobId")]
    public void Factory_WorksWithComplexObjects()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(_factory);
        var blobId = BlobId.New("my-bucket", "docs", "pdf");
        var wrapper = new BlobIdWrapper { Id = blobId, Name = "Test" };

        // Act
        var json = JsonSerializer.Serialize(wrapper, options);
        var result = JsonSerializer.Deserialize<BlobIdWrapper>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(blobId);
        result.Name.ShouldBe("Test");
    }

    private sealed class BlobIdWrapper
    {
        public BlobId? Id { get; set; }
        public string? Name { get; set; }
    }
}

