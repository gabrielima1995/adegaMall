using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The API log response.
    /// </summary>
    public class ApiLogResponse
    {
        /// <summary>
        /// The response timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The response status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The response headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The request content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The request content body.
        /// </summary>
        public string ContentBody { get; set; }

        public ApiLogResponse()
        {
        }

        public ApiLogResponse(int statusCode, Dictionary<string, string> headers, string contentType, string contentBody = null)
        {
            Timestamp = DateTime.UtcNow;
            StatusCode = statusCode;
            Headers = headers;
            ContentType = contentType;
            ContentBody = contentBody;
        }

        public static async Task<ApiLogResponse> NewAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var apiLogResponse = new ApiLogResponse
            (
                statusCode: (int)response.StatusCode,
                headers: response.Content?.Headers.ToDictionary(),
                contentType: response.Content?.Headers.ContentType.MediaType
            );

            if (response.Content != null)
            {
                await response.Content
                    .ReadAsStringAsync()
                    .ContinueWith(task =>
                    {
                        apiLogResponse.ContentBody = task.Result;
                    }, cancellationToken);
            }

            return apiLogResponse;
        }
    }
}