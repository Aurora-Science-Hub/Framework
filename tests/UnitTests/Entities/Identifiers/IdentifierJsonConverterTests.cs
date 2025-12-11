using System.Text.Json;
using AuroraScienceHub.Framework.Entities.Identifiers;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Entities.Identifiers;

/// <summary>
/// Tests for <see cref="IdentifierJsonConverter{T}"/>
/// </summary>
public sealed class IdentifierJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public IdentifierJsonConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new IdentifierJsonConverter<TestId>());
    }

    [Fact(DisplayName = "Read deserializes identifier from JSON string")]
    public void Read_DeserializesIdentifierFromJsonString()
    {
        // Arrange
        var json = "\"test-id-abc123\"";

        // Act
        var result = JsonSerializer.Deserialize<TestId>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe("test-id-abc123");
    }

    [Fact(DisplayName = "Write serializes identifier to JSON string")]
    public void Write_SerializesIdentifierToJsonString()
    {
        // Arrange
        var id = new TestId("test-id-abc123");

        // Act
        var json = JsonSerializer.Serialize(id, _options);

        // Assert
        json.ShouldBe("\"test-id-abc123\"");
    }

    [Fact(DisplayName = "WriteAsPropertyName writes identifier as property name")]
    public void WriteAsPropertyName_WritesIdentifierAsPropertyName()
    {
        // Arrange
        var dictionary = new Dictionary<TestId, string>
        {
            { new TestId("test-id-abc123"), "value" }
        };

        // Act
        var json = JsonSerializer.Serialize(dictionary, _options);

        // Assert
        json.ShouldBe("{\"test-id-abc123\":\"value\"}");
    }

    [Fact(DisplayName = "ReadAsPropertyName reads identifier from property name")]
    public void ReadAsPropertyName_ReadsIdentifierFromPropertyName()
    {
        // Arrange
        var json = "{\"test-id-abc123\":\"value\"}";

        // Act
        var result = JsonSerializer.Deserialize<Dictionary<TestId, string>>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContainKey(new TestId("test-id-abc123"));
        result[new TestId("test-id-abc123")].ShouldBe("value");
    }

    private sealed class TestId : EntityIdentifier<TestId>, IIdentifierWithPrefix
    {
        public static string Prefix => "test-id-";

        public TestId(string value) : base(value)
        {
        }
    }
}

