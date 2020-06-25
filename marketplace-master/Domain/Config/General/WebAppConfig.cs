using Application.Config;
using Newtonsoft.Json;

namespace Domain.Config.General
{
    public class WebAppConfig
    {
        public bool Enabled { get; set; } = true;

        public PushNotificationsConfig PushNotifications { get; set; } = new PushNotificationsConfig();

        [JsonIgnore]
        public bool IsEnabled => Enabled && (MaaSConfig.Current.Environment.AppUrl.StartsWith("https://") || MaaSConfig.Current.Environment.AppUrl.Contains("localhost"));
    }

    public class PushNotificationsConfig
    {
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public bool IsEnabled => MaaSConfig.Current.WebApp.Enabled && Enabled && (MaaSConfig.Current.Environment.AppUrl.StartsWith("https://") || MaaSConfig.Current.Environment.AppUrl.Contains("localhost"));

        public string GcmSenderId { get; set; } = Resources.AppResources.CONFIG_FCM_SENDER_ID;

        public string ServerKey { get; set; } = Resources.AppResources.CONFIG_FCM_SERVER_KEY;
    }
}