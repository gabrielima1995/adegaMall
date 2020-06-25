using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Application.Config.General
{
    public class UserVoiceConfig
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private string _marketplaceUrl;

        public string MarketplaceUrl
        {
            get { return _marketplaceUrl; }
            set { _marketplaceUrl = value; }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private bool _configured;

        public bool Configured
        {
            get { return _configured; }
            set { _configured = value; }
        }

        private string _loginUrl { get; set; }

        public string LoginUrl
        {
            get { return _loginUrl; }
            set { _loginUrl = value; }
        }

        public bool ShowLink { get; set; } = false;

        public void ConfigureUserVoice(bool forceRedefine = false)
        {
            if (_configured && !forceRedefine)
                return;

            if (MaaSConfig.Current.Environment.AppUrl.Contains("amazonaws") || MaaSConfig.Current.Environment.AppUrl.Contains("localhost"))
            {
                return;
            }

            string url = MaaSConfig.Current.Environment.AppUrl;

            if (string.IsNullOrWhiteSpace(url))
                return;
            if (!string.IsNullOrWhiteSpace(MaaSConfig.Current.ApplicationName) && !string.IsNullOrWhiteSpace(MaaSConfig.Current.UserVoice.DisplayName))
            {
                _marketplaceUrl = MaaSConfig.Current.Environment.AppUrl;
                _email = MaaSConfig.Current.Environment.Smtp.DefaultFromAddress;
                _configured = true;
                _loginUrl = GenerateUservoiceLink();
            }
        }

        private string GenerateUservoiceLink()
        {
            string token = TokenGenerator.Create(new UserDetails(_displayName, _email, MaaSConfig.Current.ApplicationName, _marketplaceUrl));
            return Domain.Resources.AppResources.CONFIG_USERVOICE_DEFAULT_ADDRESS + token;
        }
    }

    internal class UserDetails
    {
        [JsonProperty("guid")]
        public string Id { get; set; }

        [JsonProperty("marketplace_url")]
        public string MarketplaceUrl { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("locale")]
        public string Locale => "pt_BR";

        [JsonProperty("updates")]
        public bool Updates => true;

        [JsonProperty("comment_updates")]
        public bool CommentUpdates => true;

        public UserDetails(string userName, string email, string displayName, string marketplaceUrl)
        {
            DisplayName = displayName;
            Email = email;
            UserName = userName;
            MarketplaceUrl = marketplaceUrl;
        }
    }

    internal static class TokenGenerator
    {
        private static readonly string pw = Domain.Resources.AppResources.CONFIG_USERVOICE_DEFAULT_TOKEN;

        private static readonly string initVector = Domain.Resources.AppResources.CONFIG_USERVOICE_DEFAULT_IV;

        private const int keysize = 256;

        public static string Create(UserDetails userDetails)
        {
            var name = userDetails.UserName;
            var mail = userDetails.Email;
            var marketplaceName = userDetails.DisplayName;
            var MarketpalceUrl = userDetails.MarketplaceUrl;

            var txt = name + "," + mail + "," + marketplaceName + "," + MarketpalceUrl;

            return HttpUtility.UrlEncode(TokenGenerator.EncryptString(txt, pw));
        }

        public static string EncryptString(string plainText, string passPhrase)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var password = new PasswordDeriveBytes(passPhrase, null);
            var keyBytes = password.GetBytes(keysize / 8);

            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();

                var cipherTextBytes = memoryStream.ToArray();
                return Convert.ToBase64String(cipherTextBytes);
            }
        }
    }
}