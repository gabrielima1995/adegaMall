using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The API log entry.
    /// </summary>
    public class ApiLogEntry
    {
        /// <summary>
        /// The ID for the API log entry.
        /// </summary>
        public Guid ApiLogEntryId { get; set; }

        /// <summary>
        /// The machine that made the request.
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// The API log request.
        /// </summary>
        public ApiLogRequest Request { get; set; }

        /// <summary>
        /// The API log response.
        /// </summary>
        public ApiLogResponse Response { get; set; }

        public ApiLogEntry()
        {
            ApiLogEntryId = Guid.NewGuid();
        }

        public ApiLogEntry(string machine, ApiLogRequest request, ApiLogResponse response)
            : this()
        {
            Machine = machine;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Creates file name to blob.
        /// </summary>
        /// <returns></returns>
        public string CreateFilename()
        {
            return
                "api-request-" +
                Request.Timestamp.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) +
                "-" +
                ApiLogEntryId +
                ".json";
        }

        /// <summary>
        /// Creates stream to blob content.
        /// </summary>
        /// <returns></returns>
        public Stream ConvertToStream()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new StringSnakeCaseEnumConverter());

            var data = JsonConvert.SerializeObject(
                this,
                settings);

            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }
    }
}