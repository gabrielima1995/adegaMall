using Application.CreditCards;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Dados da página de uma nova assinatura.
    /// </summary>
    public class SubscribeViewModel
    {
        /// <summary>
        /// Tipo de assinatura.
        /// </summary>
        public SignModeViewModel SignMode { get; set; }

        /// <summary>
        /// Dados do cartão de crédito.
        /// </summary>
        public CreditCardModel CreditCard { get; set; }

        public SubscribeViewModel()
        {
            CreditCard = new CreditCardModel();
        }

        public SubscribeViewModel(SignModeViewModel signMode)
            : this()
        {
            SignMode = signMode;
        }
    }
}