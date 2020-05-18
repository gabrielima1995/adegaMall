using Framework;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.PagarMe.PaymentMethods
{
    public class PaymentMethodOperations : IPaymentMethodOperations
    {
        private ErrorDictionary Validate(ICustomer customer, bool isSubscribe)
        {
            var errors = new ErrorDictionary();

            if (customer != null && !isSubscribe)
            {
                if (string.IsNullOrEmpty(customer.GatewayId))
                {
                    errors.Add("error", nameof(customer.GatewayId) + " não pode ser vazio");
                }
            }

            return errors;
        }

        public OperationResult<ICreditCardResult> CreateCreditCard(ICustomer customer, ICreditCard creditCard, bool isSubscribe = false)
        {
            var errors = Validate(customer, isSubscribe);

            if (errors.HasErrors)
                return new OperationResult<ICreditCardResult>(errors);

            var client = new PagarMeClient();

            var request = new CreditCardRequest(client, creditCard, customer);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult<ICreditCardResult>(validationRequest);
            }

            var validation = client.Save(request);

            if (validation.OK)
            {
                return new OperationResult<ICreditCardResult>(new CreditCardResult(request));
            }

            return new OperationResult<ICreditCardResult>(validation.Errors);
        }
    }
}