using Application.Config;
using Newtonsoft.Json;
using System;

namespace Presentation.Models
{
    public class WebAppManifestVM
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("start_url")]
        public string StartUrl { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("theme_color")]
        public string ThemeColor { get; set; }

        [JsonProperty("icons")]
        public object[] Icons { get; set; }

        [JsonProperty("gcm_sender_id")]
        public string GcmSenderId { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        public WebAppManifestVM()
        {
        }

        public WebAppManifestVM(MaaSConfig config)
        {
            Name = config.ApplicationName;
            ShortName = config.ApplicationName.Truncate(30, "");
            Description = config.ApplicationName;
            GcmSenderId = "103953800507";
            StartUrl = MaaSConfig.Current.Environment.AppUrl;
            BackgroundColor = config.Layout.Colors["$color_accent1"];
            ThemeColor = config.Layout.Colors["$color_accent1"];
            Display = "minimal-ui";
            Icons = new object[3];
            Icons[0] = new
            {
                src = config.Layout.Images.Favicon,
                sizes = "196x196",
                type = "image/png",
            };
            Icons[1] = new
            {
                src = config.Layout.Images.Favicon,
                sizes = "256x256",
                type = "image/png",
            };
            Icons[2] = new
            {
                src = config.Layout.Images.Favicon,
                sizes = "512x512",
                type = "image/png",
            };
        }
    }
}