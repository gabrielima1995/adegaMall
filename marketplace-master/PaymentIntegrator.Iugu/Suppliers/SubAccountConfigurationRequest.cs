using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class SubAccountConfigurationRequest : IValidatable
    {
        [JsonProperty("commission_percent")]
        public decimal CommissionPercent { get; set; }

        [JsonProperty("bank_slip")]
        public SubAccountConfigurationBankSlip BankSlip { get; set; } = new SubAccountConfigurationBankSlip();

        [JsonProperty("credit_card")]
        public SubAccountConfigurationCreditCard CreditCard { get; set; } = new SubAccountConfigurationCreditCard();

        public SubAccountConfigurationRequest()
        {
            CommissionPercent = IuguConfig.CommissionPercent;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (CommissionPercent <= 0 || CommissionPercent > 70)
                errors.Add(nameof(CommissionPercent), nameof(CommissionPercent) + " é inválido.");

            return errors;
        }
    }

    public class SubAccountConfigurationCreditCard
    {
        public bool Active { get; set; } = true;

        public bool Installments { get; set; } = true;

        [JsonProperty("max_installments")]
        public int MaxInstallments { get; set; } = 12;
    }

    public class SubAccountConfigurationBankSlip
    {
        public bool Active { get; set; } = true;
    }
}