namespace AuroraScienceHub.Framework.Exceptions;

/// <summary>
/// Access denied exception
/// </summary>
/// <remarks>
/// All errors that occur in the process of checking permissions must be inherited from <see cref="AccessDeniedException"/>.
/// </remarks>
public class AccessDeniedException : ValidationException
{
    private const string AccessDeniedMessage = "Access denied";

    public AccessDeniedException(string message = AccessDeniedMessage, Exception? innerException = null)
        : base(message, innerException)
    { }

    public static new void ThrowIfNot(bool isValid, string message = AccessDeniedMessage)
    {
        if (!isValid)
        {
            throw new AccessDeniedException(message);
        }
    }
}
