namespace NewIntegrator.Stripe.Subscriptions
{
    public enum StripeSubscriptionStatus
    {
        Incomplete,
        IncompleteExpired,
        Trialing,
        Active,
        PastDue,
        Canceled,
        Unpaid
    }
}