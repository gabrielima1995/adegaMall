namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeWebhookData<T>
        where T : class
    {
        public T @object { get; set; }
    }
}