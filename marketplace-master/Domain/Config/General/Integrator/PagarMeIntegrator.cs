using Domain.Config.General.Integrator;
using Newtonsoft.Json;

namespace Application.Config.General.Integrator
{
    public class PagarMeIntegrator : GatewayConfig
    {
        public string ApiKey { get; set; } = Domain.Resources.AppResources.PAGARME_TEST_API_KEY;
        public string EncryptionKey { get; set; } = Domain.Resources.AppResources.PAGARME_TEST_API_ENCRYPTION_KEY;
        public string SoftDescriptor { get; set; } = "";
        public string StartupRecipient { get; set; } = Domain.Resources.AppResources.PAGARME_TEST_API_RECEIVER;
        public bool UndergoingValidation { get; set; } = false;
        public string ValidationAccountEmail { get; set; } = Domain.Resources.AppResources.PAGARME_TEST_VALIDATION_EMAIL;
        public decimal TransferCost { get; set; } = 3.67m;

        [JsonIgnore]
        public bool SellerReceivesPagarmePayment =>
            MaaSConfig.Current.Product.Enabled &&
            MaaSConfig.Current.Payment.Enabled &&
            MaaSConfig.Current.Payment.GatewayIsPagarme() &&
            (MaaSConfig.Current.Payment.PagarMe.CreditCard.Enabled || MaaSConfig.Current.Payment.PagarMe.Boleto.Enabled);
    }
}