using Newtonsoft.Json;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class PaymentTokenResponse
    {
        public string Id { get; set; }

        public string Method { get; set; }

        [JsonProperty("extra_info")]
        public PaymentTokenExtraInfo ExtraInfo { get; set; }
    }
}