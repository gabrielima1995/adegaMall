using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The API log request.
    /// </summary>
    public class ApiLogRequest
    {
        /// <summary>
        /// The request timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The request URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The request method (GET, POST, etc).
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The IP address that made the request.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The token that made the request.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The request headers.
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

        public ApiLogRequest()
        {
        }

        public ApiLogRequest(string uri, string method, string ipAddress, string token, Dictionary<string, string> headers, string contentType, string contentBody = null)
        {
            Timestamp = DateTime.UtcNow;
            Uri = uri;
            Method = method;
            IpAddress = ipAddress;
            Token = token;
            Headers = headers;
            ContentType = contentType;
            ContentBody = contentBody;
        }

        public static async Task<ApiLogRequest> NewAsync(HttpRequestMessage request, HttpContextBase context, CancellationToken cancellationToken)
        {
            var apiLogRequest = new ApiLogRequest
            (
                uri: request.RequestUri.ToString(),
                method: request.Method.Method,
                ipAddress: context.Request.UserHostAddress,
                token: request.Headers.Authorization?.Parameter,
                headers: request.Headers.ToDictionary(),
                contentType: context.Request.ContentType
            );

            if (request.Content != null)
            {
                await request.Content
                    .ReadAsStringAsync()
                    .ContinueWith(task =>
                    {
                        apiLogRequest.ContentBody = task.Result;
                    }, cancellationToken);
            }

            return apiLogRequest;
        }
    }
}