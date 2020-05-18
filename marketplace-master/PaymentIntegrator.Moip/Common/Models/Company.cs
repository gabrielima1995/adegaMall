using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Company
    {
        [JsonProperty("monthlyRevenue")]
        public string MonthlyRevenue { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("phone")]
        public Phone Phone { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("taxDocument")]
        public TaxDocument TaxDocument { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("constitutionType")]
        public string ConstitutionType { get; set; }
    }
}