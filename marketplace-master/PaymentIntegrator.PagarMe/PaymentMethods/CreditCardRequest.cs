using Framework;
using PagarMe;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.PagarMe.Common;

namespace PaymentIntegrator.PagarMe.PaymentMethods
{
    /// <summary>
    /// Representa um cartão de crédito para salvar no Pagar.me.
    /// </summary>
    public class CreditCardRequest : Card, IValidatable
    {
        public CreditCardRequest(PagarMeClient client, ICreditCard creditCard, ICustomer customer)
            : base(client.GetService())
        {
            var year = creditCard.Year;

            if (creditCard.Year > 99)
            {
                year = year % 100;
            }

            Number = creditCard.Number;
            ExpirationDate = string.Format("{0:00}{1:00}", creditCard.Month, year);
            HolderName = creditCard.FullName;
            Cvv = creditCard.CVV;

            if (customer != null)
            {
                Customer = new Customer
                {
                    Id = customer.GatewayId
                };
            }
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Number))
            {
                errors.Add(nameof(Number), nameof(Number) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(ExpirationDate))
            {
                errors.Add(nameof(ExpirationDate), nameof(ExpirationDate) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(HolderName))
            {
                errors.Add(nameof(HolderName), nameof(HolderName) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Cvv))
            {
                errors.Add(nameof(Cvv), nameof(Cvv) + " é obrigatório.");
            }

            return errors;
        }
    }
}