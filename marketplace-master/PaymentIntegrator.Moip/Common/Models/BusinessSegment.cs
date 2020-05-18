using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class BusinessSegment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}