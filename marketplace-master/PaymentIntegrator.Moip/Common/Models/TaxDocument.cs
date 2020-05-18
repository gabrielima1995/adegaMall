using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class TaxDocument
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}