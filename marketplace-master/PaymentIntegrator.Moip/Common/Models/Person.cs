using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Person
    {
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public Phone Phone { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("taxDocument")]
        public TaxDocument TaxDocument { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }
    }
}