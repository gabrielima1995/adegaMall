using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Iugu.Common;
using System.Linq;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class CreditCard : IValidatable
    {
        public string Number { get; }

        [JsonProperty("verification_value")]
        public string VerificationValue { get; }

        [JsonProperty("first_name")]
        public string FirstName { get; }

        [JsonProperty("last_name")]
        public string LastName { get; }

        public string Month { get; }

        public string Year { get; }

        public CreditCard(ICreditCard creditCard)
        {
            Number = creditCard.Number;
            VerificationValue = creditCard.CVV;

            if (!string.IsNullOrWhiteSpace(creditCard.FullName))
            {
                var splitName = creditCard.FullName.Trim().Split(new[] { ' ' }, 2);
                FirstName = splitName.First();
                LastName = splitName.Last();
            }

            Month = creditCard.Month.ToString("00");
            Year = creditCard.Year.ToString();
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Number))
            {
                errors.Add(nameof(Number), nameof(Number) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(VerificationValue))
            {
                errors.Add(nameof(VerificationValue), nameof(VerificationValue) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                errors.Add(nameof(FirstName), nameof(FirstName) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                errors.Add(nameof(LastName), nameof(LastName) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Month))
            {
                errors.Add(nameof(Month), nameof(Month) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Year))
            {
                errors.Add(nameof(Year), nameof(Year) + " é obrigatório.");
            }

            return errors;
        }
    }
}