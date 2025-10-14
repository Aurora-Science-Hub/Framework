namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Extensions for <see cref="Exception"/>
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Returns the full message of the given exception including all inner exceptions
    /// </summary>
    public static string GetFullMessage(this Exception e)
    {
        var exceptions = e.GetInnerExceptions();
        var messages = exceptions.Select(x => x.Message);
        return string.Join(Environment.NewLine, messages);
    }

    /// <summary>
    /// Returns all inner exceptions of the given exception
    /// </summary>
    /// <remarks>
    /// If the given exception is an <see cref="AggregateException"/>, all inner exceptions are returned.
    /// </remarks>
    public static List<Exception> GetInnerExceptions(this Exception e)
    {
        List<Exception> exceptions = [];

        if (e is AggregateException exception)
        {
            exceptions.AddRange(exception.InnerExceptions);
        }
        else
        {
            exceptions.Add(e);
        }

        var innerExceptions = exceptions
            .Where(i => i.InnerException != null)
            .SelectMany(i => i.InnerException.Required().GetInnerExceptions())
            .ToList();

        exceptions.AddRange(innerExceptions);

        return exceptions;
    }
}
