using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;

namespace PaymentIntegrator.Iugu.Customers
{
    public class CustomerOperations : ICustomerOperations
    {
        public OperationResult Create(ICustomer customer, IAddress address)
        {
            var client = new IuguClient("/customers");

            var request = new CustomerRequest(customer);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult(validationRequest);
            }

            var validation = client.Post<CustomerResponse>(request);

            if (validation.OK)
            {
                var response = validation.Result;

                customer.SetGatewayId(response.Id);
            }

            return validation;
        }

        public OperationResult Update(ICustomer customer, IAddress address)
        {
            var validation = new OperationResult<CustomerResponse>();

            if (string.IsNullOrEmpty(customer.GatewayId))
            {
                validation.AddError("error", "GatewayId é obrigatório");

                return validation;
            }

            var client = new IuguClient("/customers/" + customer.GatewayId);

            var request = new CustomerRequest(customer);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult(validationRequest);
            }

            validation = client.Put<CustomerResponse>(request);

            return validation;
        }
    }
}