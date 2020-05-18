using Framework;
using NewIntegrator.Stripe.Customers;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;

namespace NewIntegrator.Stripe.Retro.Customers
{
    public class StripeRetroCustomerOperations : ICustomerOperations
    {
        public OperationResult Create(ICustomer customer, IAddress address)
        {
            var service = new StripeCustomerService();

            var c = service.Create(new StripeCustomerCreate
            {
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.PhoneNumber
            });

            customer.SetGatewayId(c.Id);

            return OperationResult.Success;
        }

        public OperationResult Update(ICustomer customer, IAddress address)
        {
            var service = new StripeCustomerService();

            service.Update(customer.GatewayId, new StripeCustomerUpdate
            {
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.PhoneNumber
            });

            return OperationResult.Success;
        }
    }
}