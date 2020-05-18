using Newtonsoft.Json;
using System;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class BankAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("agencyNumber")]
        public string AgencyNumber { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("holder")]
        public Holder Holder { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("accountCheckNumber")]
        public string AccountCheckNumber { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("agencyCheckNumber")]
        public string AgencyCheckNumber { get; set; }

        [JsonProperty("bankNumber")]
        public string BankNumber { get; set; }
    }
}