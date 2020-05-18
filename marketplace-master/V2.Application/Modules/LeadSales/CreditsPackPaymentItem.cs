using PaymentIntegrator.Core.Payments;

namespace V2.Application.Modules.LeadSales
{
    public class CreditsPackPaymentItemVM : IPaymentItem
    {
        public string Item { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get; set; }

        public bool PriceRequired => true;

        public decimal SubTotal { get; set; }
    }
}