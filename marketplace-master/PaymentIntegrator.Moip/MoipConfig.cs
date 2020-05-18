namespace PaymentIntegrator.Moip
{
    public class MoipConfig
    {
        static MoipConfig()
        {
            Current = new MoipConfig();
        }

        public static MoipConfig Current { get; private set; }

        public string AccountKey { get; set; }
        public string AccountToken { get; set; }
        public string AppId { get; set; }
        public string AppAccessToken { get; set; }
        public string AppSecret { get; set; }
        public string PublicEncryptionKey { get; set; }
        public bool MarketplaceZeroCommission { get; set; }
        public bool UseMarketplaceName { get; set; }
        public string StartupReceiverId { get; set; }

        public string StatementDescription { get; set; }
        public string PostbackUrl { get; set; }
        public string NotificationRule { get; set; }

        public bool IsSandbox { get; set; }
        public bool PaymentWarrantyEnabled { get; set; }
        public bool RecipientChargeProcessingFee { get; set; }
    }
}
