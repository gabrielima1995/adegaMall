using Framework;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class PaymentMethodOperations : IPaymentMethodOperations
    {
        private OperationResult<PaymentTokenResponse> CreatePaymentToken(ICreditCard creditCard)
        {
            var client = new IuguClient("/payment_token");

            var request = new PaymentTokenRequest(IuguConfig.AccountId, IuguConfig.IsTest, creditCard);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult<PaymentTokenResponse>(validationRequest);
            }

            var validation = client.Post<PaymentTokenResponse>(request);

            return validation;
        }

        private OperationResult<PaymentMethodResponse> CreatePaymentMethod(PaymentTokenResponse paymentToken, ICustomer customer)
        {
            var client = new IuguClient($"/customers/{customer.GatewayId}/payment_methods");

            var request = new PaymentMethodRequest(paymentToken, customer);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult<PaymentMethodResponse>(validationRequest);
            }

            var validation = client.Post<PaymentMethodResponse>(request);

            return validation;
        }

        private ErrorDictionary Validate(ICustomer customer)
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(customer.GatewayId))
            {
                errors.Add("error", nameof(customer.GatewayId) + " não pode ser vazio");
            }

            return errors;
        }

        public OperationResult<ICreditCardResult> CreateCreditCard(ICustomer customer, ICreditCard creditCard, bool isSubscribe = false)
        {
            var errors = Validate(customer);

            if (errors.HasErrors)
                return new OperationResult<ICreditCardResult>(errors);

            var validationToken = CreatePaymentToken(creditCard);

            if (validationToken.OK)
            {
                var validationMethod = CreatePaymentMethod(validationToken.Result, customer);

                if (validationMethod.OK)
                {
                    return new OperationResult<ICreditCardResult>(new CreditCardStatus(validationToken.Result, validationMethod.Result));
                }

                return new OperationResult<ICreditCardResult>(validationMethod.Errors);
            }

            return new OperationResult<ICreditCardResult>(validationToken.Errors);
        }
    }
}