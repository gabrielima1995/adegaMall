using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Phone
    {
        [JsonProperty("areaCode")]
        public string AreaCode { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }
}