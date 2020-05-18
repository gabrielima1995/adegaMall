using Application.Config.Common;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configurações gerais de pedidos.
    /// </summary>
    public class OrderConfig
    {
        /// <summary>
        /// Valor mínimo de cada compra na plataforma.
        /// </summary>
        public decimal MinOrderTotal { get; set; } = 0M;

        /// <summary>
        /// Valor múnimo de compra por vendedor.
        /// </summary>
        public decimal MinOrderBySeller { get; set; } = 0M;

        /// <summary>
        /// Determina se é possível realizar o pagamento por fora da plataforma.
        /// </summary>
        public bool OfflinePaymentEnabled { get; set; } = false;

        /// <summary>
        /// Configuração do campo texto para o comprador especificar informações adicionais no pedido, a partir do carrinho.
        /// </summary>
        public RegisterConfig ExtraInfoField { get; set; } = new RegisterConfig(enabled: false, mandatory: false);

        /// <summary>
        /// Determina se a plataforma deve enviar uma cópia do email de nova venda para os adminstradores.
        /// </summary>
        public bool SendOrderCopyToAdmins { get; set; } = true;

        /// <summary>
        /// Mostra o endereço do vendedor caso o pagamento já foi efetuado ou se o pagamento for por fora de plataforma.
        /// </summary>
        public bool ShowAddressAfterPayment { get; set; } = false;
    }
}
