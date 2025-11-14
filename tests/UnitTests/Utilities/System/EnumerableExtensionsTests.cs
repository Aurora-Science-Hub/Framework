using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Unit tests for <see cref="EnumerableExtensions"/>.
/// </summary>
public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void WhereNotNull_WithReferenceTypes_FiltersOutNulls()
    {
        // Arrange
        var source = new List<string?> { "a", null, "b", null, "c" };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { "a", "b", "c" });
    }

    [Fact]
    public void WhereNotNull_WithReferenceTypes_EmptySource_ReturnsEmpty()
    {
        // Arrange
        var source = new List<string?>();

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereNotNull_WithReferenceTypes_AllNulls_ReturnsEmpty()
    {
        // Arrange
        var source = new List<string?> { null, null, null };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereNotNull_WithReferenceTypes_NoNulls_ReturnsAll()
    {
        // Arrange
        var source = new List<string?> { "a", "b", "c" };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { "a", "b", "c" });
    }

    [Fact]
    public void WhereNotNull_WithReferenceTypes_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<string?> source = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => source.WhereNotNull().ToArray());
    }

    [Fact]
    public void WhereNotNull_WithReferenceTypes_UsesLazyEvaluation()
    {
        // Arrange
        var callCount = 0;
        IEnumerable<string?> Source()
        {
            callCount++;
            yield return "a";
            callCount++;
            yield return null;
            callCount++;
            yield return "b";
        }

        // Act
        var result = Source().WhereNotNull();

        // Assert - не должно быть вызовов до перечисления
        callCount.ShouldBe(0);

        // Act - перечисляем
        var materialized = result.ToArray();

        // Assert
        callCount.ShouldBe(3);
        materialized.ShouldBeEquivalentTo(new[] { "a", "b" });
    }


    [Fact]
    public void WhereNotNull_WithValueTypes_FiltersOutNulls()
    {
        // Arrange
        var source = new List<int?> { 1, null, 2, null, 3 };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void WhereNotNull_WithValueTypes_EmptySource_ReturnsEmpty()
    {
        // Arrange
        var source = new List<int?>();

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereNotNull_WithValueTypes_AllNulls_ReturnsEmpty()
    {
        // Arrange
        var source = new List<int?> { null, null, null };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void WhereNotNull_WithValueTypes_NoNulls_ReturnsAll()
    {
        // Arrange
        var source = new List<int?> { 1, 2, 3 };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void WhereNotNull_WithValueTypes_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<int?> source = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => source.WhereNotNull().ToArray());
    }

    [Fact]
    public void WhereNotNull_WithValueTypes_UsesLazyEvaluation()
    {
        // Arrange
        var callCount = 0;
        IEnumerable<int?> Source()
        {
            callCount++;
            yield return 1;
            callCount++;
            yield return null;
            callCount++;
            yield return 2;
        }

        // Act
        var result = Source().WhereNotNull();

        // Assert - should not be called before enumeration
        callCount.ShouldBe(0);

        // Act - enumerate
        var materialized = result.ToArray();

        // Assert
        callCount.ShouldBe(3);
        materialized.ShouldBeEquivalentTo(new[] { 1, 2 });
    }

    [Fact(DisplayName = "WhereNotNull with DateTime filters out nulls")]
    public void WhereNotNull_WithDateTime_FiltersOutNulls()
    {
        // Arrange
        var date1 = new DateTime(2021, 1, 1);
        var date2 = new DateTime(2021, 1, 2);
        var source = new List<DateTime?> { date1, null, date2 };

        // Act
        var result = source.WhereNotNull().ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { date1, date2 });
    }
}

