using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class SubAccountRequest : IValidatable
    {
        public string Name { get; }

        [JsonProperty("commission_percent")]
        public decimal CommissionPercent { get; }

        public SubAccountRequest(ISupplier supplier)
        {
            Name = supplier.Name;
            CommissionPercent = IuguConfig.CommissionPercent;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (CommissionPercent <= 0 || CommissionPercent > 70)
            {
                errors.Add(nameof(CommissionPercent), nameof(CommissionPercent) + " é inválido.");
            }

            return errors;
        }
    }
}