namespace Presentation.Models.SubscriptionViewModels
{
    public class StripeSubscriptionRequest
    {
        public string card_token { get; set; }

        public string payment_intent_id { get; set; }
    }
}