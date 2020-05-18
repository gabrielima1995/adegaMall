using Newtonsoft.Json;
using System;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class Account
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("person")]
        public Person Person { get; set; }

        [JsonProperty("transparentAccount")]
        public bool TransparentAccount { get; set; }

        [JsonProperty("monthlyRevenueId")]
        public int MonthlyRevenueId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("email")]
        public Email Email { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("businessSegment")]
        public BusinessSegment BusinessSegment { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("feeType")]
        public string FeeType { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("softDescriptor")]
        public string SoftDescriptor { get; set; }
    }
}