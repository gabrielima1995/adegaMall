using Application.Config;
using Newtonsoft.Json;

namespace Domain.Config.General.Integrator
{
    public class MoipConfig : GatewayConfig
    {
        public MoipOptionsConfig Options { get; set; } = new MoipOptionsConfig();
        public string AccountKey { get; set; } = Resources.AppResources.MOIP_TEST_ACCOUNT_KEY;
        public string AccountToken { get; set; } = Resources.AppResources.MOIP_TEST_ACCOUNT_TOKEN;
        public string AppId { get; set; }
        public string AppAccessToken { get; set; }
        public string AppSecret { get; set; }
        public string StartupReceiverId { get; set; }
        public string PublicEncryptionKey { get; set; }
        public string NotificationRuleId { get; set; }
        public bool MarketplaceZeroCommission { get; set; } = false;
        public bool IsSandbox { get; set; } = true;

        [JsonIgnore]
        public bool SellerReceivesMoipPayment =>
            MaaSConfig.Current.Product.Enabled &&
            MaaSConfig.Current.Payment.Enabled &&
            MaaSConfig.Current.Payment.GatewayIsWirecard() &&
            (MaaSConfig.Current.Payment.Moip.CreditCard.Enabled || MaaSConfig.Current.Payment.Moip.Boleto.Enabled);
    }

    public class MoipOptionsConfig
    {
        public bool CustomerPaysFees { get; set; } = false;
        public MoipFeesConfig FeesConfig { get; set; } = new MoipFeesConfig();
    }

    public class MoipFeesConfig
    {
        public decimal BaseFee { get; set; } = 5.49M;
        public decimal FixedFee { get; set; } = 0.69M;

        public decimal[] InstallmentFees = new decimal[12]
        {
             0m,  //1x
             0m,  //2x
             0m,  //3x
             0m,  //4x
             0m,  //5x
             0m,  //6x
             0m,  //7x
             0m,  //8x
             0m, //9x
             0m, //10x
             0m, //11x
             0m  //12x
        };

        public decimal[] AnticipationFees = new decimal[13]
        {
            0.00m,  //0x
            0.00m,  //1x
            4.50m,  //2x
            5.00m,  //3x
            5.50m,  //4x
            6.50m,  //5x
            7.50m,  //6x
            8.50m,  //7x
            9.50m,  //8x
            10.50m, //9x
            11.50m, //10x
            12.00m, //11x
            12.50m  //12x
        };
    }
}