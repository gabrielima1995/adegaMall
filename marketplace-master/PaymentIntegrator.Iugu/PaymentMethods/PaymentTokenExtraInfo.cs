using Newtonsoft.Json;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class PaymentTokenExtraInfo
    {
        public string Brand { get; set; }

        [JsonProperty("holder_name")]
        public string HolderName { get; set; }

        [JsonProperty("display_number")]
        public string DisplayNumber { get; set; }
    }
}