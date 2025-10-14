namespace AuroraScienceHub.Framework.Exceptions;

/// <summary>
/// Unexpected exception
/// </summary>
public class UnexpectedException : FrameworkException
{
    public UnexpectedException(string message, Exception? innerException = null)
        : base(message, innerException)
    { }

    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new UnexpectedException(message);
        }
    }

    public static void ThrowIfNot(bool condition, string message)
    {
        if (!condition)
        {
            throw new UnexpectedException(message);
        }
    }

    public static T ThrowIfNull<T>(T? argument, string message)
        where T : class
        => argument ?? throw new UnexpectedException(message);

    public static T ThrowIfNull<T>(T? argument, string message)
        where T : struct
        => argument ?? throw new UnexpectedException(message);

    public static string ThrowIfWhiteSpace(string? argument, string message)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new UnexpectedException(message);
        }

        return argument;
    }
}
