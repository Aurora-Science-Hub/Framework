namespace AuroraScienceHub.Framework.Exceptions;

/// <summary>
/// Validation exception
/// </summary>
public class ValidationException : FrameworkException
{
    private const string DefaultMessage = "Invalid data";

    public readonly ValidationExceptionReason Reason;

    public ValidationException(ValidationExceptionReason reasons)
        : this(DefaultMessage, reasons)
    {
    }

    public ValidationException(string message, Exception? innerException = null)
        : this(message, new ValidationExceptionReason(), innerException)
    {
    }

    public ValidationException(string message, ValidationExceptionReason reason, Exception? innerException = null)
        : base(message, innerException)
    {
        Reason = reason;
    }

    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message);
        }
    }

    public static void ThrowIfNot(bool condition, string message)
    {
        if (!condition)
        {
            throw new ValidationException(message);
        }
    }

    public static T ThrowIfNull<T>(T? argument, string message)
        where T : class
        => argument ?? throw new ValidationException(message);

    public static T ThrowIfNull<T>(T? argument, string message)
        where T : struct
        => argument ?? throw new ValidationException(message);
}

