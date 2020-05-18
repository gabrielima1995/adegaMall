using PaymentIntegrator.Core.Payments;

namespace PaymentIntegrator.Iugu.Payments
{
    public class PaymentItem : IPaymentItem
    {
        public string Item { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Amount => Price * Quantity;

        public bool PriceRequired => true;

        public decimal SubTotal { get; set; }
    }
}