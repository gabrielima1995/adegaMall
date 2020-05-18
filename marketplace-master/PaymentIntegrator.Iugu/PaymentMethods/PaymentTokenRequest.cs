using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class PaymentTokenRequest : IValidatable
    {
        [JsonProperty("account_id")]
        public string AccountId { get; }

        public string Method { get; }

        public bool Test { get; }

        public CreditCard Data { get; }

        public PaymentTokenRequest(string accountId, bool test, ICreditCard creditCard)
        {
            Method = "credit_card";
            AccountId = accountId;
            Test = test;
            Data = new CreditCard(creditCard);
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(AccountId))
            {
                errors.Add(nameof(AccountId), nameof(AccountId) + " é obrigatório.");
            }

            errors.Merge(Data.Validate());

            return errors;
        }
    }
}