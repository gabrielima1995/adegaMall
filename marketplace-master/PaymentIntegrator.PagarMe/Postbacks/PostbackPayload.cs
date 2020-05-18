using System.Web;

namespace PaymentIntegrator.PagarMe.Postbacks
{
    public class PostbackPayload
    {
        public string id { get; set; }

        public string fingerprint { get; set; }

        public string @event { get; set; }

        public string old_status { get; set; }

        public string desired_status { get; set; }

        public string current_status { get; set; }

        public static PostbackPayload Parse(string queryString)
        {
            var nameValue = HttpUtility.ParseQueryString(queryString);

            var payload = new PostbackPayload
            {
                id = nameValue["id"],
                fingerprint = nameValue["fingerprint"],
                @event = nameValue["event"],
                old_status = nameValue["old_status"],
                desired_status = nameValue["desired_status"],
                current_status = nameValue["current_status"]
            };

            return payload;
        }
    }
}
