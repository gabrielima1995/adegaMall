using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The webhook log response.
    /// </summary>
    public class WebhookLogResponse
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

        public WebhookLogResponse()
        {
        }

        public WebhookLogResponse(int statusCode, Dictionary<string, string> headers, string contentType, string contentBody = null)
        {
            Timestamp = DateTime.UtcNow;
            StatusCode = statusCode;
            Headers = headers;
            ContentType = contentType;
            ContentBody = contentBody;
        }

        public static WebhookLogResponse New(HttpWebResponse response)
        {
            var webhookLogResponse = new WebhookLogResponse
            (
                statusCode: (int)response.StatusCode,
                headers: response.Headers.ToDictionary(),
                contentType: response.ContentType
            );

            using (var s = response.GetResponseStream())
            {
                if (s != null)
                {
                    using (var sw = new StreamReader(s))
                    {
                        webhookLogResponse.ContentBody = sw.ReadToEnd();
                    }
                }
            }

            return webhookLogResponse;
        }
    }
}