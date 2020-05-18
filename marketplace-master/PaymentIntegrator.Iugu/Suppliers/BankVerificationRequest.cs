using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class BankVerificationRequest : IValidatable
    {
        public string Agency { get; }

        public string Account { get; }

        [JsonProperty("account_type")]
        public string AccountType { get; }

        public string Bank { get; }

        [JsonProperty("automatic_validation")]
        public bool AutomaticValidation { get; }

        public BankVerificationRequest(IBankAccount bankAccount)
        {
            Agency = bankAccount.Branch;
            Account = bankAccount.Number;
            AccountType = bankAccount.AccountType == Core.Common.AccountType.ContaCorrente
                ? "cc"
                : "cp";
            Bank = string.Format("{0:000}", (int)bankAccount.Bank);
            AutomaticValidation = true;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Agency))
            {
                errors.Add(nameof(Agency), nameof(Agency) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Account))
            {
                errors.Add(nameof(Account), nameof(Account) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(AccountType))
            {
                errors.Add(nameof(AccountType), nameof(AccountType) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Bank))
            {
                errors.Add(nameof(Bank), nameof(Bank) + " é obrigatório.");
            }

            return errors;
        }
    }
}