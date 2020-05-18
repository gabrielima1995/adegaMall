using System;

namespace Infra.Crosscutting.Email.SendGrid
{
    public class SendGridEmailConfig : IEmailConfig
    {
        public string FromAddress { get; }

        public string FromDisplayName { get; }

        public string UserName { get; }

        public string Password { get; }

        private SendGridEmailConfig(string fromAddress, string fromDisplayName, string userName, string password)
        {
            FromAddress = fromAddress;
            FromDisplayName = fromDisplayName;
            UserName = userName;
            Password = password;
        }

        public static SendGridEmailConfig Get()
        {
            return new SendGridEmailConfig(
                ConfigHelper.GetSetting("SENDGRID:FROM:ADDRESS"),
                ConfigHelper.GetSetting("SENDGRID:FROM:DISPLAY_NAME"),
                ConfigHelper.GetSetting("SENDGRID:USER"),
                ConfigHelper.GetSetting("SENDGRID:PASSWORD"));
        }
    }
}