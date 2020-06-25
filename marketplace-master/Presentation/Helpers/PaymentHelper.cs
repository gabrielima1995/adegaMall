using Application.Config;
using System;
using System.Configuration;

namespace Presentation.Helpers
{
    public static class PaymentHelper
    {
        private static string ActiveGateway => MaaSConfig.Current.Payment.ActiveGateway;
        private static bool IsIugu
        {
            get
            {
                return "iugu".Equals(ActiveGateway, StringComparison.OrdinalIgnoreCase);
            }
        }
        private static bool IsPagarMe
        {
            get
            {
                return "pagarme".Equals(ActiveGateway, StringComparison.OrdinalIgnoreCase);
            }
        }
        private static bool IsMoip
        {
            get
            {
                return "moip".Equals(ActiveGateway, StringComparison.OrdinalIgnoreCase);
            }
        }
        private static string TestUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WEBHOOK_TEST_URL"];
            }
        }

        public static string GetBoletoPostbackUrl()
        {
            //if (true) return TestUrl;

            if (IsIugu)
                return ComposeUrl("/iugu/boletopostback");

            if (IsPagarMe)
                return ComposeUrl("/pagarme/boletopostback");

            if (IsMoip)
                return ComposeUrl("/moip/boletopostpack");

            return null;
        }

        public static string GetSubscriptionPostbackUrl()
        {
            //if (true) return TestUrl;

            if (IsIugu)
                return ComposeUrl("/iugu/subscriptionpostback");

            if (IsPagarMe)
                return ComposeUrl("/pagarme/subscriptionpostback");

            if (IsMoip)
                return ComposeUrl("/moip/subscriptionpostback");

            return null;
        }

        private static string ComposeUrl(string relativeUrl)
        {
            var baseUrl = new Uri(MaaSConfig.Current.Environment.AppUrl);
            var uri = new Uri(baseUrl, relativeUrl);
            return uri.ToString();
        }

        public static decimal FromCents(this int i)
        {
            return i / 100M;
        }
    }
}