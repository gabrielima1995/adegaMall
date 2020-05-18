using PagarMe;
using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.PagarMe.PaymentMethods
{
    public static class CreditCardHelper
    {
        public static Card AsPagarmeCard(this ICreditCardResult creditCardStatus, PagarMeClient client)
        {
            return new Card(client.GetService())
            {
                Id = creditCardStatus.ReferenceId
            };
        }
    }
}
