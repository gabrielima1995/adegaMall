using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class RequestReceivedApiLog : BaseEntity<Guid>
    {
        public string IP { get; private set; }

        public string Token { get; private set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateTime { get; private set; }

        public string Url { get; private set; }

        public string HttpVerb { get; private set; }

        public string HttpStatusCode { get; private set; }

        private RequestReceivedApiLog()
        {
        }

        public static RequestReceivedApiLog New(Guid id, string ip, string token, DateTime dateTime, string url, string httpVerb, string httpStatusCode)
        {
            return new RequestReceivedApiLog
            {
                Id = id,
                IP = ip,
                Token = token,
                DateTime = dateTime,
                Url = url,
                HttpVerb = httpVerb,
                HttpStatusCode = httpStatusCode
            };
        }
    }
}