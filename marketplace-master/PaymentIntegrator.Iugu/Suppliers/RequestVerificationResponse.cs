using Newtonsoft.Json;
using System;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class RequestVerificationResponse
    {
        public string Id { get; set; }

        [JsonProperty("data")]
        public RequestVerification Data { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}