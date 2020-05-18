using PagarMe;
using PaymentIntegrator.Core.Helpers;

namespace PaymentIntegrator.PagarMe
{
    /// <summary>
    /// Classe de configuração dos parãmetros do Pagar.me.
    /// </summary>
    public static class PagarMeConfig
    {
        private static string _startupRecipient;

        /// <summary>
        /// Chave da api.
        /// </summary>
        public static string ApiKey
        {
            get { return PagarMeService.DefaultApiKey = PagarMeService.DefaultApiKey ?? ConfigHelper.GetSetting("PAGARME:API_KEY"); }
            set { PagarMeService.DefaultApiKey = value; }
        }

        /// <summary>
        /// Chave de encriptação.
        /// </summary>
        public static string EncryptionKey
        {
            get { return PagarMeService.DefaultEncryptionKey = PagarMeService.DefaultEncryptionKey ?? ConfigHelper.GetSetting("PAGARME:ENCRYPTION_KEY"); }
            set { PagarMeService.DefaultEncryptionKey = value; }
        }

        /// <summary>
        /// Descrição que aparecerá na fatura depois do nome da loja. Máximo de 13 caracteres. Padrão: (vazio).
        /// </summary>
        public static string SoftDescriptor { get; set; } = ConfigHelper.GetSetting("PAGARME:SOFT_DESCRIPTOR", "");

        /// <summary>
        /// Indica se o recebedor vinculado a regra de divisão será cobrado pelas taxas da transação.
        /// Padrão: false (as taxas são cobradas somente da startup). Para que as taxas sejam cobradas da startup e do(s) recebedor(es), proporcionalmente, informar true.
        /// </summary>
        public static bool RecipientChargeProcessingFee { get; set; } = ConfigHelper.GetSetting("PAGARME:RECIPIENT:CHARGE_PROCESSING_FEE", false);

        /// <summary>
        /// Indica se o recebedor vinculado a regra de divisão assumirá o risco da transação, ou seja, possíveis estornos (chargeback).
        /// Padrão: false (somente a startup fica responsável pelo chargeback). Para que a startup e o(s) recebedor(es) assumam o risco, informar true.
        /// </summary>
        public static bool RecipientLiable { get; set; } = ConfigHelper.GetSetting("PAGARME:RECIPIENT:LIABLE", false);

        /// <summary>
        /// Frequência na qual o recebedor irá ser pago. Valores possíveis: daily, weekly, monthly. Padrão: daily.
        /// </summary>
        public static string RecipientTransferInterval { get; set; } = ConfigHelper.GetSetting("PAGARME:RECIPIENT:TRANSFER_INTERVAL", "daily");

        /// <summary>
        /// Dia no qual o recebedor vai ser pago. Padrão: 0.
        /// </summary>
        public static int RecipientTransferDay { get; set; } = ConfigHelper.GetSetting("PAGARME:RECIPIENT:TRANSFER_DAY", 0);

        /// <summary>
        /// Indica se o recebedor pode receber os pagamentos automaticamente. Padrão: true.
        /// </summary>
        public static bool RecipientTransferEnabled { get; set; } = ConfigHelper.GetSetting("PAGARME:RECIPIENT:TRANSFER_ENABLED", true);

        /// <summary>
        /// Taxa descontada do recebedor que ficará para a startup (percentual). Pode ser usado em conjunto com PAGARME:STARTUP:AMOUNT_FEE.
        /// </summary>
        public static decimal StartupPercentageFee { get; set; } = ConfigHelper.GetSetting("PAGARME:STARTUP:PERCENTAGE_FEE", 0M);

        /// <summary>
        /// Taxa descontada do recebedor que ficará para a startup (em reais). Pode ser usado em conjunto com PAGARME:STARTUP:PERCENTAGE_FEE.
        /// </summary>
        public static decimal StartupAmountFee { get; set; } = ConfigHelper.GetSetting("PAGARME:STARTUP:AMOUNT_FEE", 0);

        /// <summary>
        /// Recebedor (conta) padrão da startup, onde serão distribuídas as taxas descontadas dos recebedores.
        /// Para descobrir, chame a url https://api.pagar.me/1/company/?api_key=[API_KEY] e veja o valor de default_recipient_id:
        /// existe uma conta para testes e outra para produção.
        /// </summary>
        public static string StartupRecipient
        {
            get { return _startupRecipient = _startupRecipient ?? ConfigHelper.GetSetting("PAGARME:STARTUP:DEFAULT_RECIPIENT"); }
            set { _startupRecipient = value; }
        }

        /// <summary>
        /// Indica se o mecanismo de antifraude está habilitado no Pagar.me.
        /// Padrão: false (desabilitado).
        /// </summary>
        public static bool AntiFraudEnabled { get; set; } = ConfigHelper.GetSetting("PAGARME:ANTI_FRAUD:ENABLE", false);

        public static int TransferCostCents { get; set; } = ConfigHelper.GetSetting<int>("PAGARME:TRANSFER_COST_CENTS", 367);
    }
}