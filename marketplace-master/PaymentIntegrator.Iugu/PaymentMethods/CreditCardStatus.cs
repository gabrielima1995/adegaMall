using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.Iugu.PaymentMethods
{
    public class CreditCardStatus : ICreditCardResult
    {
        public string ReferenceId { get; private set; }

        public string Token { get; private set; }

        public string DisplayNumber { get; private set; }

        public string Brand { get; private set; }

        public string HolderName { get; private set; }

        public CreditCardStatus(PaymentTokenResponse paymentToken, PaymentMethodResponse paymentMethod)
        {
            ReferenceId = paymentMethod.Id;
            Token = paymentToken.Id;
            DisplayNumber = paymentToken.ExtraInfo.DisplayNumber;
            Brand = paymentToken.ExtraInfo.Brand;
            HolderName = paymentToken.ExtraInfo.HolderName;
        }
    }
}