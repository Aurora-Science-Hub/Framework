using AuroraScienceHub.Framework.Utilities.System;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.Utilities.System;

/// <summary>
/// Tests for <see cref="AuroraScienceHub.Framework.Utilities.System.ExceptionExtensions"/>.
/// </summary>
public sealed class ExceptionExtensionsTests
{
    [Fact]
    public void GetFullMessage_WhenExceptionWithInnerExceptions_ReturnsFullMessage()
    {
        // Arrange
        var exception = new Exception("Outer exception",
            new Exception("Inner exception 1",
                new Exception("Inner exception 2")));

        // Act
        var result = exception.GetFullMessage();

        // Assert
        result.ShouldBe("Outer exception" + Environment.NewLine +
                           "Inner exception 1" + Environment.NewLine +
                           "Inner exception 2");
    }

    [Fact]
    public void GetInnerExceptions_WhenExceptionWithInnerExceptions_ReturnsInnerExceptions()
    {
        // Arrange
        var exception = new Exception("Outer exception",
            new Exception("Inner exception 1",
                new Exception("Inner exception 2")));

        // Act
        var result = exception.GetInnerExceptions();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Message.ShouldBe("Outer exception");
        result[1].Message.ShouldBe("Inner exception 1");
        result[2].Message.ShouldBe("Inner exception 2");
    }

    [Fact]
    public void GetInnerExceptions_WhenAggregateException_ReturnsInnerExceptions()
    {
        // Arrange
        var exception = new AggregateException(
            new Exception("Inner exception 1"),
            new Exception("Inner exception 2"));

        // Act
        var result = exception.GetInnerExceptions();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Message.ShouldBe("Inner exception 1");
        result[1].Message.ShouldBe("Inner exception 2");
    }

    [Fact]
    public void GetInnerExceptions_WhenExceptionWithInnerAggregateException_ReturnsInnerExceptions()
    {
        // Arrange
        var exception = new Exception("Outer exception",
            new AggregateException(
                new Exception("Inner exception 1"),
                new Exception("Inner exception 2")));

        // Act
        var result = exception.GetInnerExceptions();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Message.ShouldBe("Outer exception");
        result[1].Message.ShouldBe("Inner exception 1");
        result[2].Message.ShouldBe("Inner exception 2");
    }
}
