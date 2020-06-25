namespace Domain.Config.General
{
    public class EnvironmentConfig
    {
        public string AppUrl { get; set; } = "http://localhost:44340/";
        public bool DisableAutoRedirect { get; set; } = false;
        public CloudStorageConfig CloudStorage { get; set; } = new CloudStorageConfig();
        public GoogleServicesConfig GoogleServices { get; set; } = new GoogleServicesConfig();
        public SmtpConfig Smtp { get; set; } = new SmtpConfig();
        public FullCalendarConfig FullCalendar { get; set; } = new FullCalendarConfig();

        public class CloudStorageConfig
        {
            public string Endpoint { get; set; } = Resources.AppResources.CONFIG_STORAGE_DEFAULT_ENDPOINT;
            public string Username { get; set; } = Resources.AppResources.CONFIG_STORAGE_DEFAULT_USERNAME;
            public string Password { get; set; } = Resources.AppResources.CONFIG_STORAGE_DEFAULT_PASSWORD;
        }

        public class SmtpConfig
        {
            public string DefaultFromAddress { get; set; } = Resources.AppResources.CONFIG_SMTP_DEFAULT_FROM_ADDRESS;
            public string DefaultFromName { get; set; } = Resources.AppResources.CONFIG_SMTP_DEFAULT_FROM_NAME;
            public string Host { get; set; } = Resources.AppResources.CONFIG_SMTP_DEFAULT_HOSTNAME;
            public int Port { get; set; } = 587;
            public string Username { get; set; } = Resources.AppResources.CONFIG_SMTP_DEFAULT_USERNAME;
            public string Password { get; set; } = Resources.AppResources.CONFIG_SMTP_DEFAULT_PASSWORD;
            public bool UseSsl { get; set; }
        }

        public class GoogleServicesConfig
        {
            public string TagManagerId { get; set; }
            public string AnalyticsAccountId { get; set; }
            public string MapsApiKey { get; set; } = Resources.AppResources.CONFIG_GMAPS_DEFAULT_APIKEY;
            public string MapsApiKeyUnrestricted { get; set; } = Resources.AppResources.CONFIG_GMAPS_DEFAULT_APIKEY_UNRESTRICTED;
        }

        public class FullCalendarConfig
        {
            public bool HasPremiumLicense { get; set; }

            public string LicenseKey { get; set; }
        }
    }
}