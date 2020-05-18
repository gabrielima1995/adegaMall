using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Holder
    {
        [JsonProperty("taxDocument")]
        public TaxDocument TaxDocument { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }
    }
}