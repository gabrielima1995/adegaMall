using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Payments
{
    /// <summary>
    /// Representa um item de uma fatura.
    /// </summary>
    public class InvoiceItem : IValidatable
    {
        /// <summary>
        /// Descrição do item.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Quantidade do item.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Preço do item, em centavos. Valores negativos entram como desconto no total.
        /// </summary>
        [JsonProperty("price_cents")]
        public int PriceCents { get; }

        public InvoiceItem(IPaymentItem paymentItem)
        {
            Description = paymentItem.Item;
            Quantity = (int)paymentItem.Quantity;
            PriceCents = (int)(paymentItem.Price * 100);
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Description))
                errors.Add(nameof(Description), nameof(Description) + " é obrigatório.");

            if (Quantity <= 0)
                errors.Add(nameof(Quantity), nameof(Quantity) + " é obrigatório.");

            if (PriceCents == 0)
                errors.Add(nameof(PriceCents), nameof(PriceCents) + " é obrigatório.");

            return errors;
        }
    }
}