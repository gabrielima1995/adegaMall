using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.Core.Payments
{
    public interface IPaymentOptions
    {
        int Installments { get; set; }

        PaymentMethod Method { get; set; }

        ICreditCardResult CreditCardResult { get; set; }

        ICreditCard CreditCard { get; set; }

        string PostbackUrl { get; set; }
    }
}
