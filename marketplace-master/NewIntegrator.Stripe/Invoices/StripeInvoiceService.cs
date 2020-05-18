using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.Invoices
{
    public class StripeInvoiceService
    {
        public async Task<StripeInvoice> PayAsync(string invoiceId)
        {
            var service = new InvoiceService();

            var options = new InvoicePayOptions
            {
                Expand = new List<string>
                {
                    "payment_intent"
                }
            };

            var invoice = await service.PayAsync(invoiceId, options);

            return invoice.FromStripe();
        }
    }
}