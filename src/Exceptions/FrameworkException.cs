namespace AuroraScienceHub.Framework.Exceptions;

/// <summary>
/// Platform exception
/// </summary>
/// <remarks>
/// All errors that occur in the application must be inherited from <see cref="FrameworkException"/>.
/// </remarks>
public abstract class FrameworkException(string? message, Exception? innerException = null)
    : Exception(message, innerException);
