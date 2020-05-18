using System;
using System.Configuration;
using System.Globalization;

namespace PaymentIntegrator.Iugu
{
    public static class IuguConfig
    {
        private static string _accountId;
        private static string _apiKeyLive;
        private static string _apiKeyTest;
        private static string _baseUrl;
        private static decimal _commissionPercent = -1;
        private static string _mode;
        private static string _softDescriptor;

        public static string AccountId
        {
            get { return _accountId = _accountId ?? ConfigurationManager.AppSettings["IUGU:ACCOUNT_ID"]; }
            set { _accountId = value; }
        }

        public static string ApiKeyLive
        {
            get { return _apiKeyLive = _apiKeyLive ?? ConfigurationManager.AppSettings["IUGU:API_KEY:LIVE"]; }
            set { _apiKeyLive = value; }
        }

        public static string ApiKeyTest
        {
            get { return _apiKeyTest = _apiKeyTest ?? ConfigurationManager.AppSettings["IUGU:API_KEY:TEST"]; }
            set { _apiKeyTest = value; }
        }

        public static string BaseUrl
        {
            get { return _baseUrl = _baseUrl ?? "https://api.iugu.com/v1"; }
            set { _baseUrl = value; }
        }

        public static decimal CommissionPercent
        {
            get { return _commissionPercent = _commissionPercent == -1 ? Convert.ToDecimal(ConfigurationManager.AppSettings["IUGU:COMMISSION"], CultureInfo.InvariantCulture) : _commissionPercent; }
            set { _commissionPercent = value; }
        }

        public static bool IsTest
        {
            get { return !Mode.Equals("live", StringComparison.InvariantCultureIgnoreCase); }
        }

        public static string Mode
        {
            get { return _mode = _mode ?? ConfigurationManager.AppSettings["IUGU:MODE"]; }
            set { _mode = value; }
        }

        public static string PostbackUrl { get; set; }

        public static string SoftDescriptor
        {
            get { return _softDescriptor = _softDescriptor ?? ConfigurationManager.AppSettings["IUGU:SOFT_DESCRIPTOR"]; }
            set { _softDescriptor = value; }
        }
    }
}