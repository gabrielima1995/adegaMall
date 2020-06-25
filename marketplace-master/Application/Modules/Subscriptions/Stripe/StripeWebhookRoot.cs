namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeWebhookRoot<T>
        where T : class
    {
        public StripeWebhookData<T> data { get; set; }
    }
}