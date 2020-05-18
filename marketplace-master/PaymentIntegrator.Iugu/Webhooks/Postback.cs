using System;
using System.Web;

namespace PaymentIntegrator.Iugu.Webhooks
{
    public class Postback
    {
        public EventType EventType { get; set; }

        public string Id { get; set; }

        public string Status { get; set; }

        public string AccountId { get; set; }

        public bool IsPaid()
        {
            return "paid".Equals(Status, StringComparison.OrdinalIgnoreCase);
        }

        public Postback()
        {
        }

        public Postback(string postContent)
        {
            ParsePostContent(postContent);
        }

        private void ParsePostContent(string postContent)
        {
            if (postContent == null)
                throw new ArgumentNullException(postContent);

            var queryString = HttpUtility.ParseQueryString(postContent);

            EventType = EventTypeHelper.ParseDescription(queryString["event"]);
            Id = queryString["data[id]"];
            Status = queryString["data[status]"];
            AccountId = queryString["data[account_id]"];
        }
    }
}
