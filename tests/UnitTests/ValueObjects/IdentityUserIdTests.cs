using System.Text.Json;
using AuroraScienceHub.Framework.ValueObjects.Accounting;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.ValueObjects;

public sealed class IdentityUserIdTests
{
    [Fact(DisplayName = "A user identifier is serialized as a JSON string in a DTO")]
    public void Serialize_Dto_WritesString()
    {
        // Arrange
        var dto = new { userId = new IdentityUserId("Realm:User/A") };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        json.ShouldBe("""{"userId":"Realm:User/A"}""");
    }

    [Fact(DisplayName = "A non-GUID user subject is read without normalization")]
    public void Deserialize_Subject_PreservesValue()
    {
        // Arrange
        const string json = """ " Realm:User/A " """;

        // Act
        var id = JsonSerializer.Deserialize<IdentityUserId>(json);

        // Assert
        id.ShouldBe(new IdentityUserId(" Realm:User/A "));
    }

    [Fact(DisplayName = "A user identifier can round-trip as a dictionary key")]
    public void Serialize_Dictionary_RoundTripsKey()
    {
        // Arrange
        var id = new IdentityUserId("Realm:User/A");
        var dictionary = new Dictionary<IdentityUserId, int> { [id] = 42 };

        // Act
        var result = JsonSerializer.Deserialize<Dictionary<IdentityUserId, int>>(
            JsonSerializer.Serialize(dictionary));

        // Assert
        result.ShouldNotBeNull()[id].ShouldBe(42);
    }
}
