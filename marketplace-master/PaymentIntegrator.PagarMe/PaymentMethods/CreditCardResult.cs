using PagarMe;
using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.PagarMe.PaymentMethods
{
    /// <summary>
    /// Representa um cartão de crédito já salvo no Pagar.me.
    /// </summary>
    public class CreditCardResult : ICreditCardResult
    {
        public string ReferenceId { get; }

        public string Token { get; }

        public string DisplayNumber { get; }

        public string Brand { get; }

        public string HolderName { get; }

        public CreditCardResult(Card card)
        {
            ReferenceId = card.Id;
            Token = card.Id;
            DisplayNumber = card.LastDigits;
            Brand = card.Brand.ToString();
            HolderName = card.HolderName;
        }
    }
}