using Application.Config;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Presentation.Areas.Admin.Models
{
    public class PaymentGatewayConfigViewModel
    {
        public PagarmeConfigViewModel PagarMeConfig { get; set; } = new PagarmeConfigViewModel();
        public MoipConfigViewModel MoipConfig { get; set; } = new MoipConfigViewModel();

        public PaymentGatewayConfigViewModel()
        {
        }

        public bool IsGatewayConfigured()
        {
            switch (MaaSConfig.Current.Payment.ActiveGateway)
            {
                case "pagarme":
                    return PagarMeConfig.IsConfigured();

                case "moip":
                    return MoipConfig.IsConfigured();

                default:
                    return false;
            }
        }

        public PaymentGatewayConfigViewModel(MaaSConfig config)
        {
            if (config.Payment.GatewayIsPagarme())
            {
                PagarMeConfig = new PagarmeConfigViewModel()
                {
                    ApiKey = config.Payment.PagarMe.ApiKey,
                    EncryptionKey = config.Payment.PagarMe.EncryptionKey,
                    StartupReceiver = config.Payment.PagarMe.StartupRecipient,
                };
            }
            else if (config.Payment.GatewayIsWirecard())
            {
                MoipConfig = new MoipConfigViewModel()
                {
                    AccountKey = config.Payment.Moip.AccountKey,
                    AccountToken = config.Payment.Moip.AccountToken,
                    PublicEncryptionKey = config.Payment.Moip.PublicEncryptionKey,
                };
            }
        }
    }

    public class PagarmeConfigViewModel
    {
        [Required]
        [DisplayName("Chave de Api")]
        public string ApiKey { get; set; }

        [Required]
        [DisplayName("Chave de Encriptação")]
        public string EncryptionKey { get; set; }

        [Required]
        [DisplayName("Id do Recebedor da Startup")]
        public string StartupReceiver { get; set; }

        public void Sanitize()
        {
            ApiKey = ApiKey.Trim();
            EncryptionKey = EncryptionKey.Trim();
            StartupReceiver = StartupReceiver.Trim();
        }

        public bool Validate()
        {
            bool isValid = true;

            isValid &= Regex.IsMatch(ApiKey, @"^ak_live_\w+$");
            isValid &= Regex.IsMatch(EncryptionKey, @"^ek_live_\w+$");
            isValid &= Regex.IsMatch(StartupReceiver, @"^re_\w+$");

            return isValid;
        }

        public bool IsConfigured()
        {
            Sanitize();
            var isConfigured = true;

            isConfigured &= !string.IsNullOrEmpty(ApiKey);
            isConfigured &= !string.IsNullOrEmpty(EncryptionKey);
            isConfigured &= !string.IsNullOrEmpty(StartupReceiver);
            isConfigured &= Validate();

            return isConfigured;
        }
    }

    public class MoipConfigViewModel
    {
        [Required]
        [DisplayName("Chave de acesso")]
        public string AccountKey { get; set; }

        [Required]
        [DisplayName("Token de acesso")]
        public string AccountToken { get; set; }

        [Required]
        [DisplayName("Chave pública de encriptação")]
        public string PublicEncryptionKey { get; set; }

        public MoipConfigViewModel()
        {
        }

        public bool IsConfigured()
        {
            return !MaaSConfig.Current.Payment.Moip.IsSandbox && !string.IsNullOrEmpty(MaaSConfig.Current.Payment.Moip.AppId);
        }

        public void Sanitize()
        {
            AccountKey = AccountKey.Trim();
            AccountToken = AccountToken.Trim();
            PublicEncryptionKey = PublicEncryptionKey.Trim();
        }

        public bool Validate()
        {
            bool isValid = true;

            isValid &= Regex.IsMatch(AccountKey, @"[\dA-z]{32,48}");
            isValid &= Regex.IsMatch(AccountToken, @"[\dA-z]{26,36}");
            isValid &= Regex.IsMatch(PublicEncryptionKey, @"-----BEGIN PUBLIC KEY-----[\S\n]+-----END PUBLIC KEY-----");

            return isValid;
        }
    }
}