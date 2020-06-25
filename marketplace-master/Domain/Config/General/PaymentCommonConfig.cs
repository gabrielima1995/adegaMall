namespace Domain.Config.General
{
    public class PaymentCommonConfig
    {
        /// <summary>
        /// Indica se o recebedor vinculado a regra de divisão será cobrado pelas taxas da transação.
        /// </summary>
        public bool RecipientChargeProcessingFee { get; set; } = false;

        /// <summary>
        /// Frequência na qual o recebedor irá ser pago. Valores possíveis: daily, weekly, monthly.
        /// </summary>
        public string RecipientTransferInterval { get; set; } = "daily";

        /// <summary>
        /// Dia no qual o recebedor vai ser pago.
        /// </summary>
        public int RecipientTransferDay { get; set; } = 0;

        /// <summary>
        /// Indica se o recebedor pode receber os pagamentos automaticamente.
        /// </summary>
        public bool RecipientTransferEnabled { get; set; } = true;

        /// <summary>
        /// Taxa descontada do recebedor que ficará para a startup (percentual).
        /// </summary>
        public decimal GlobalStartupPercentageFee { get; set; } = 20M;

        /// <summary>
        /// Taxa descontada do recebedor que ficará para a startup (em reais).
        /// </summary>
        public decimal GlobalStartupAmountFee { get; set; } = 0M;

        /// <summary>
        /// Indica se o recebedor vinculado a regra de divisão assumirá o risco da transação, ou seja, possíveis estornos (chargeback).
        /// </summary>
        public bool GlobalIsLiable { get; set; } = false;

        /// <summary>
        /// Habilita a opção de definir
        /// </summary>
        public bool EnableCustomFeesByVendor { get; set; } = true;

        /// <summary>
        /// Habilita transação em garantia
        /// </summary>
        public bool EnableTransactionWarranty { get; set; } = false;

        /// <summary>
        /// Número de dias para extensão automática do prazo para entrega
        /// </summary>
        public int DeadlineAutoExtensionDays { get; set; } = 14;

        /// <summary>
        /// Quantos dias antes do pedido ser concluído o vendedor deve ser notificado
        /// </summary>
        public int OrderConclusionNotificationDays { get; set; } = 3;

        /// <summary>
        /// Quando habilitado, valor do frete é repassado para a Marketplace ao invés do Vendedor
        /// </summary>
        public bool EnableShippingSplitToMarketplace { get; set; } = false;

        /// <summary>
        /// Utilizado para configurar qual será o nome na fatura
        /// </summary>
        public StatementDescriptor StatementDescriptor { get; set; } = new StatementDescriptor();
    }
}
