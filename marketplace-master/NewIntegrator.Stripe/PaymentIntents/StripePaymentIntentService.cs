using Stripe;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.PaymentIntents
{
    public class StripePaymentIntentService
    {
        public async Task<StripePaymentIntent> CreateManualAsync(string paymentMethodId, long amountCents, string currency, string description)
        {
            var paymentIntentService = new PaymentIntentService();

            var options = new PaymentIntentCreateOptions
            {
                PaymentMethodId = paymentMethodId,
                Amount = amountCents,
                Currency = currency.ToLower(),
                ConfirmationMethod = "manual",
                Confirm = true,
                Description = description
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options);

            return paymentIntent.FromStripe();
        }

        public async Task<StripePaymentIntent> ConfirmManualAsync(string paymentIntentId)
        {
            var paymentIntentService = new PaymentIntentService();

            var options = new PaymentIntentConfirmOptions();

            var paymentIntent = await paymentIntentService.ConfirmAsync(paymentIntentId, options);

            return paymentIntent.FromStripe();
        }

        public async Task<StripePaymentIntent> GetAsync(string paymentIntentId)
        {
            var paymentIntentService = new PaymentIntentService();

            var options = new PaymentIntentGetOptions();

            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, options);

            return paymentIntent.FromStripe();
        }
    }
}