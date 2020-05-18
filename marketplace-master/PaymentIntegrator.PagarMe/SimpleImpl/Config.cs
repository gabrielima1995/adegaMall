namespace PaymentIntegrator.PagarMe.SimpleImpl
{
    public static class Config
    {
        public static string BasePath { get; set; }
        public static string ApiKey { get; set; }
        public static string EncryptionKey { get; set; }

        static Config()
        {
            BasePath = "https://api.pagar.me/1";
        }
    }
}
