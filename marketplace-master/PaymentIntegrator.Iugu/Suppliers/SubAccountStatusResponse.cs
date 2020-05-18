using Newtonsoft.Json;
using PaymentIntegrator.Core.Suppliers;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class SubAccountStatusResponse : ISupplierStatus
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("is_verified?")]
        public bool IsVerified { get; set; }

        [JsonProperty("last_verification_request_status")]
        public string Status { get; set; }

        [JsonProperty("last_verification_request_feedback")]
        public string Feedback { get; set; }
    }
}