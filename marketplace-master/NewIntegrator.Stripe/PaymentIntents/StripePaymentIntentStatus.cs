namespace NewIntegrator.Stripe.PaymentIntents
{
    public enum StripePaymentIntentStatus
    {
        RequiresPaymentMethod,
        RequiresConfirmation,
        RequiresAction,
        Processing,
        RequiresCapture,
        Canceled,
        Succeeded
    }
}