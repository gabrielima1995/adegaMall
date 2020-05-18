using Stripe;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.Customers
{
    public class StripeCustomerService
    {
        public StripeCustomer Create(StripeCustomerCreate create)
        {
            var service = new CustomerService();

            var options = new CustomerCreateOptions
            {
                Email = create.Email,
                Name = create.Name,
                Phone = create.Phone
            };

            if (!string.IsNullOrWhiteSpace(create.Source))
            {
                options.Source = create.Source;
            }

            var customer = service.Create(options);

            return customer.FromStripe();
        }

        public StripeCustomer Update(string customerId, StripeCustomerUpdate update)
        {
            var service = new CustomerService();

            var options = new CustomerUpdateOptions
            {
                Email = update.Email,
                Name = update.Name,
                Phone = update.Phone
            };

            if (!string.IsNullOrWhiteSpace(update.Source))
            {
                options.Source = update.Source;
            }

            var customer = service.Update(customerId, options);

            return customer.FromStripe();
        }

        public async Task<StripeCustomer> ChangeSourceAsync(string customerId, string source)
        {
            var service = new CustomerService();

            var options = new CustomerUpdateOptions
            {
                Source = source
            };

            var customer = await service.UpdateAsync(customerId, options);

            return customer.FromStripe();
        }
    }
}