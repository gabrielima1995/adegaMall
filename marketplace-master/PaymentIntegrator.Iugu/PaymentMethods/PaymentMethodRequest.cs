using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class PaymentMethodRequest : IValidatable
    {
        [JsonProperty("customer_id")]
        public string CustomerId { get; }

        public string Description { get; }

        public string Token { get; }

        public PaymentMethodRequest(PaymentTokenResponse paymentToken, ICustomer customer)
        {
            CustomerId = customer.GatewayId;
            Description = "Cartão de crédito de " + customer.Name;
            Token = paymentToken.Id;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(CustomerId))
            {
                errors.Add(nameof(CustomerId), nameof(CustomerId) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Description))
            {
                errors.Add(nameof(Description), nameof(Description) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Token))
            {
                errors.Add(nameof(Token), nameof(Token) + " é obrigatório.");
            }

            return errors;
        }
    }
}