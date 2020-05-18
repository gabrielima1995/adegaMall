using Stripe;
using System;

namespace NewIntegrator.Stripe.PaymentIntents
{
    internal static class PaymentIntentExtensions
    {
        public static StripePaymentIntent FromStripe(this PaymentIntent intent)
        {
            return new StripePaymentIntent
            {
                Id = intent.Id,
                Status = intent.EnumerationStatus(),
                ClientSecret = intent.ClientSecret,
                Amount = intent.Amount ?? 0
            };
        }

        public static StripePaymentIntentStatus EnumerationStatus(this PaymentIntent intent)
        {
            switch (intent.Status)
            {
                case "requires_payment_method":
                    return StripePaymentIntentStatus.RequiresPaymentMethod;

                case "requires_confirmation":
                    return StripePaymentIntentStatus.RequiresConfirmation;

                case "requires_action":
                    return StripePaymentIntentStatus.RequiresAction;

                case "processing":
                    return StripePaymentIntentStatus.Processing;

                case "requires_capture":
                    return StripePaymentIntentStatus.RequiresCapture;

                case "canceled":
                    return StripePaymentIntentStatus.Canceled;

                case "succeeded":
                    return StripePaymentIntentStatus.Succeeded;

                default:
                    throw new ArgumentOutOfRangeException(nameof(intent.Status));
            }
        }
    }
}