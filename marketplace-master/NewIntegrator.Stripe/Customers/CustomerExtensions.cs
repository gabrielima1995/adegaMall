using Stripe;

namespace NewIntegrator.Stripe.Customers
{
    internal static class CustomerExtensions
    {
        public static StripeCustomer FromStripe(this Customer customer)
        {
            return new StripeCustomer
            {
                Id = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone
            };
        }
    }
}