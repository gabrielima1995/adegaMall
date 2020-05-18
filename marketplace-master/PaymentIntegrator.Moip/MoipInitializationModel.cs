namespace PaymentIntegrator.Moip
{
    public class MoipInitializationModel
    {
        private readonly string applicationName;
        private readonly string description;
        private readonly string siteUrl;
        private readonly string redirectUrl;

        public MoipInitializationModel(string applicationName, string description, string siteUrl, string redirectUrl)
        {
            this.applicationName = applicationName;
            this.description = description;
            this.siteUrl = siteUrl;
            this.redirectUrl = redirectUrl;
        }

        internal object GetJsonForApiRequest()
        {
            var name = applicationName;

            if (name.Length > 60)
                name = name.Substring(0, 60);

            return new
            {
                name,
                description,
                site = siteUrl,
                redirectUri = redirectUrl
            };
        }
    }
}