using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PaymentIntegrator.PagarMe.SimpleImpl.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Transaction
    {
        public string Object { get; set; }
        public long Id { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
        public int Amount { get; set; }
        public int Cost { get; set; }
        public string PostbackUrl { get; set; }
        public string BoletoUrl { get; set; }
        public string BoletoBarcode { get; set; }
    }
}
