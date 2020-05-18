using Newtonsoft.Json;

namespace NewIntegrator.Stripe.PaymentIntents
{
    public class ConfirmPaymentRequest
    {
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonProperty("payment_intent_id")]
        public string PaymentIntentId { get; set; }
    }
}