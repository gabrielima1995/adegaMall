using System;

namespace PaymentIntegrator.PagarMe.Postbacks
{
    public class Postback
    {
        public string @object { get; set; }

        public string status { get; set; }

        public string model { get; set; }

        public string model_id { get; set; }

        public string headers { get; set; }

        public string payload { get; set; }

        public string request_url { get; set; }

        public string retries { get; set; }

        public DateTime date_created { get; set; }

        public DateTime? date_updated { get; set; }

        public string signature { get; set; }

        public string id { get; set; }

        public PostbackPayload ParsePayload()
        {
            if (payload != null)
            {
                try
                {
                    return PostbackPayload.Parse(payload);
                }
                catch { }
            }

            return null;
        }
    }
}
