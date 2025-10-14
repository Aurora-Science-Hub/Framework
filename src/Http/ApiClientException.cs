using System.Net;

namespace AuroraScienceHub.Framework.Http;

/// <summary>
/// Represents an exception thrown by an API client.
/// </summary>
public sealed class ApiClientException : HttpRequestException
{
    public ApiClientException()
    {
    }

    public ApiClientException(ProblemDetails problemDetails)
        : base(
            message: $"{problemDetails.Title}: {problemDetails.Detail}",
            inner: null,
            statusCode: (HttpStatusCode?)problemDetails.Status)
    {
        ProblemDetails = problemDetails;
    }

    public ProblemDetails? ProblemDetails { get; set; }
}
