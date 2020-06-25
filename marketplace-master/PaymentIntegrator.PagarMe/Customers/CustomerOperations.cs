using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;

namespace PaymentIntegrator.PagarMe.Customers
{
    public class CustomerOperations : ICustomerOperations
    {
        private OperationResult Save(ICustomer customer, IAddress address)
        {
            var client = new PagarMeClient();

            var request = new CustomerRequest(client, customer, address);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult(validationRequest);
            }

            var validation = client.Save(request);

            if (validation.OK)
            {
                customer.SetGatewayId(request.Id);
            }

            return validation;
        }

        public OperationResult Create(ICustomer customer, IAddress address)
        {
            return Save(customer, address);
        }

        public OperationResult Update(ICustomer customer, IAddress address)
        {
            // Não implementado no Pagar.me.

            return new OperationResult();
        }
    }
}