using AuroraScienceHub.Framework.Utilities;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities;

/// <summary>
/// Unit tests for <see cref="DateTimeRange"/>.
/// </summary>
public sealed class DateTimeRangeTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange
        var start = new DateTime(2021, 1, 1);
        var end = new DateTime(2021, 1, 2);

        // Act
        var range = new DateTimeRange(start, end);

        // Assert
        range.Start.ShouldBe(start);
        range.End.ShouldBe(end);
    }

    [Fact]
    public void Contructor_WhenStartAfterEnd_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2021, 1, 2);
        var end = new DateTime(2021, 1, 1);

        // Act
        Action act = () => new DateTimeRange(start, end);

        // Assert
        Should.Throw<ArgumentException>(act)
            .ParamName.ShouldBe("start");
    }

    [Fact]
    public void Deconstruct_ReturnsStartAndEndDates()
    {
        // Arrange
        var start = new DateTime(2021, 1, 1);
        var end = new DateTime(2021, 1, 2);
        var range = new DateTimeRange(start, end);

        // Act
        var (actualStart, actualEnd) = range;

        // Assert
        actualStart.ShouldBe(start);
        actualEnd.ShouldBe(end);
    }

    [Fact]
    public void Merge_WhenNull_ReturnsOriginalRange()
    {
        // Arrange
        var start = new DateTime(2021, 1, 1);
        var end = new DateTime(2021, 1, 2);
        var range = new DateTimeRange(start, end);

        // Act
        var mergedRange = range.Merge(null);

        // Assert
        mergedRange.ShouldBe(range);
    }

    [Fact]
    public void Merge_WhenOverlappingRange_ReturnsMergedRange()
    {
        // Arrange
        var start1 = new DateTime(2021, 1, 1);
        var end1 = new DateTime(2021, 1, 2);
        var range1 = new DateTimeRange(start1, end1);

        var start2 = new DateTime(2021, 1, 1, 12, 0, 0);
        var end2 = new DateTime(2021, 1, 3);
        var range2 = new DateTimeRange(start2, end2);

        // Act
        var mergedRange = range1.Merge(range2);

        // Assert
        mergedRange.Start.ShouldBe(start1);
        mergedRange.End.ShouldBe(end2);
    }
}
