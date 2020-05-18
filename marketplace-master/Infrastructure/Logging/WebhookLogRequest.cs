using System;
using System.Collections.Generic;
using System.Net;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The webhook log request.
    /// </summary>
    public class WebhookLogRequest
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

        public WebhookLogRequest()
        {
        }

        public WebhookLogRequest(string uri, string method, string token, Dictionary<string, string> headers, string contentType, string contentBody = null)
        {
            Timestamp = DateTime.UtcNow;
            Uri = uri;
            Method = method;
            Token = token;
            Headers = headers;
            ContentType = contentType;
            ContentBody = contentBody;
        }

        public static WebhookLogRequest New(HttpWebRequest request, string token, string contentBody)
        {
            var webhookLogRequest = new WebhookLogRequest
            (
                uri: request.RequestUri.ToString(),
                method: request.Method,
                token: token,
                headers: request.Headers.ToDictionary(),
                contentType: request.ContentType,
                contentBody: contentBody
            );

            return webhookLogRequest;
        }
    }
}