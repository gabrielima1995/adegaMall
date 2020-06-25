using Application.Config.General.Integrator;

namespace Domain.Config.General.Integrator
{
    public class GatewayConfig
    {
        /// <summary>
        /// Configurações de parcelamento de compras.
        /// </summary>
        public GatewayInstallmentsConfig Installments { get; set; } = new GatewayInstallmentsConfig();

        /// <summary>
        /// Configurações de cartão de crédito.
        /// </summary>
        public GatewayCreditCardConfig CreditCard { get; set; } = new GatewayCreditCardConfig(enabled: true);

        /// <summary>
        /// Configurações de boleto.
        /// </summary>
        public GatewayBankslipConfig Boleto { get; set; } = new GatewayBankslipConfig();
    }
}
