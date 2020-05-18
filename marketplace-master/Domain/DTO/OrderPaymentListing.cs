using PaymentIntegrator.Core.PaymentMethods;

namespace Domain.DTO
{
    public class OrderPaymentListing
    {
        public PaymentMethod Method { get; set; }

        public int Installments { get; set; }

        public bool IsPaid { get; set; }
    }
}