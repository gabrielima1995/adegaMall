using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Config.General
{
    public class SocialNetworksConfig
    {
        public FacebookSocialNetworkConfig Facebook { get; set; } = new FacebookSocialNetworkConfig();
        public InstagramSocialNetworkConfig Instagram { get; set; } = new InstagramSocialNetworkConfig();
        public YouTubeSocialNetworkConfig YouTube { get; set; } = new YouTubeSocialNetworkConfig();
        public VimeoSocialNetworkConfig Vimeo { get; set; } = new VimeoSocialNetworkConfig();
        public LinkedInSocialNetworkConfig LinkedIn { get; set; } = new LinkedInSocialNetworkConfig();
        public TwitterSocialNetworkConfig Twitter { get; set; } = new TwitterSocialNetworkConfig();
        public PinterestSocialNetworkConfig Pinterest { get; set; } = new PinterestSocialNetworkConfig();
        public WhatsAppSocialNetworkConfig WhatsApp { get; set; } = new WhatsAppSocialNetworkConfig();

        public bool UseBlackIcons { get; set; } = false;
        public bool UseWhiteIcons { get; set; } = false;

        [JsonIgnore]
        public bool HasAny => Networks.Any(network => network.IsConfigured);
        [JsonIgnore]
        public IReadOnlyCollection<SocialNetworkConfig> Networks => (new SocialNetworkConfig[] { Facebook, Instagram, YouTube, Vimeo, LinkedIn, Twitter, Pinterest, WhatsApp }).ToList().AsReadOnly();


        public class FacebookSocialNetworkConfig : SocialNetworkConfig
        {
            public FacebookSocialNetworkConfig() : base("Facebook", "social-icons-facebook") { }
        }

        public class InstagramSocialNetworkConfig : SocialNetworkConfig
        {
            public InstagramSocialNetworkConfig() : base("Instagram", "social-icons-instagram") { }
        }

        public class YouTubeSocialNetworkConfig : SocialNetworkConfig
        {
            public YouTubeSocialNetworkConfig() : base("YouTube", "social-icons-youtube") { }
        }

        public class VimeoSocialNetworkConfig : SocialNetworkConfig
        {
            public VimeoSocialNetworkConfig() : base("Vimeo", "social-icons-vimeo") { }
        }

        public class LinkedInSocialNetworkConfig : SocialNetworkConfig
        {
            public LinkedInSocialNetworkConfig() : base("LinkedIn", "social-icons-linkedin") { }
        }

        public class TwitterSocialNetworkConfig : SocialNetworkConfig
        {
            public TwitterSocialNetworkConfig() : base("Twitter", "social-icons-twitter") { }
        }

        public class PinterestSocialNetworkConfig : SocialNetworkConfig
        {
            public PinterestSocialNetworkConfig() : base("Pinterest", "social-icons-pinterest") { }
        }

        public class WhatsAppSocialNetworkConfig : SocialNetworkConfig
        {
            public WhatsAppSocialNetworkConfig() : base("WhatsApp", "social-icons-whatsapp") { }
        }
    }

    public class SocialNetworkConfig
    {
        private string url = "www.ideianoar.com.br";

        public SocialNetworkConfig(string name, string cssClass)
        {
            Name = name;
            CssClass = cssClass;
        }

        public string Url { get => url; set => url = SanitizeUrl(value); }

        [JsonIgnore]
        public bool IsConfigured => Url != null;

        [JsonIgnore]
        public string Name { get; }

        [JsonIgnore]
        public string CssClass { get; }

        private string SanitizeUrl(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            input = input.Trim();

            if (!input.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                input = "https://" + input;

            if (!Uri.TryCreate(input, UriKind.Absolute, out Uri parsed))
                return null;

            return parsed.ToString();
        }
    }


}