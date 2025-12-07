using System.Text.Json;
using AuroraScienceHub.Framework.Entities.Identifiers;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Entities.Identifiers;

/// <summary>
/// Tests for <see cref="IdentifierJsonConverterFactory"/>
/// </summary>
public sealed class IdentifierJsonConverterFactoryTests
{
    private readonly IdentifierJsonConverterFactory _factory;

    public IdentifierJsonConverterFactoryTests()
    {
        _factory = new IdentifierJsonConverterFactory();
    }

    [Fact(DisplayName = "CanConvert returns true for IIdentifier types")]
    public void CanConvert_WhenIdentifierType_ReturnsTrue()
    {
        // Arrange
        var type = typeof(TestId);

        // Act
        var result = _factory.CanConvert(type);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact(DisplayName = "CanConvert returns false for non-IIdentifier types")]
    public void CanConvert_WhenNonIdentifierType_ReturnsFalse()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = _factory.CanConvert(type);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact(DisplayName = "CreateConverter returns converter for identifier type")]
    public void CreateConverter_WhenIdentifierType_ReturnsConverter()
    {
        // Arrange
        var options = new JsonSerializerOptions();

        // Act
        var converter = _factory.CreateConverter(typeof(TestId), options);

        // Assert
        converter.ShouldNotBeNull();
        converter.ShouldBeOfType<IdentifierJsonConverter<TestId>>();
    }

    [Fact(DisplayName = "Factory serializes and deserializes identifier correctly")]
    public void Factory_SerializesAndDeserializesIdentifier()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(_factory);
        var id = new TestId("test-id-abc123");

        // Act
        var json = JsonSerializer.Serialize(id, options);
        var result = JsonSerializer.Deserialize<TestId>(json, options);

        // Assert
        json.ShouldBe("\"test-id-abc123\"");
        result.ShouldNotBeNull();
        result.Value.ShouldBe("test-id-abc123");
    }

    [Fact(DisplayName = "Factory caches converter for same type")]
    public void Factory_CachesConverterForSameType()
    {
        // Arrange
        var options = new JsonSerializerOptions();

        // Act
        var converter1 = _factory.CreateConverter(typeof(TestId), options);
        var converter2 = _factory.CreateConverter(typeof(TestId), options);

        // Assert
        converter1.ShouldNotBeNull();
        converter2.ShouldNotBeNull();
        converter1.GetType().ShouldBe(converter2.GetType());
    }

    private sealed class TestId : EntityIdentifier<TestId>, IIdentifierWithPrefix
    {
        public static string Prefix => "test-id-";

        public TestId(string value) : base(value)
        {
        }
    }
}

