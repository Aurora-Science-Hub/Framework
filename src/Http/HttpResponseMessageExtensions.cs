using System.Net.Mime;
using AuroraScienceHub.Framework.Json;

namespace AuroraScienceHub.Framework.Http;

/// <summary>
/// Extension methods for <see cref="HttpResponseMessage"/>
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Ensures that the response is successful.
    /// </summary>
    /// <Exception cref="ApiClientException">Thrown if the response is not successful and contains a problem details object.</Exception>
    /// <Exception cref="HttpRequestException">Thrown if the response is not successful and does not contain a problem details object.</Exception>
    public static async Task EnsureSuccess(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.ProblemJson)
        {
            var json = await response.Content.ReadAsStringAsync();

            var problemDetails = DefaultJsonSerializer.Deserialize<ProblemDetails>(json);
            if (problemDetails is not null)
            {
                throw new ApiClientException(problemDetails);
            }
            response.EnsureSuccessStatusCode();
        }
        else
        {
            response.EnsureSuccessStatusCode();
        }
    }
}
