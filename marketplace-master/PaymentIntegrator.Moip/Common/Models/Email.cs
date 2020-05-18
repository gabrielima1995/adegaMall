using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Email
    {
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}