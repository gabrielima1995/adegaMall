using System.Configuration;

namespace Infrastructure
{
    public static class AwsConfig
    {
        public static string S3BucketName =>
            GetValue("S3:BUCKETNAME");

        public static string S3AccessKey =>
            GetValue("S3:ACCESSKEY");

        public static string S3SecretKey =>
            GetValue("S3:SECRETKEY");

        public static bool UseAws =>
            HasValue(S3BucketName) &&
            HasValue(S3AccessKey) &&
            HasValue(S3SecretKey);

        private static string GetValue(string key) =>
            ConfigurationManager.AppSettings[key];

        private static bool HasValue(string configValue) =>
            !string.IsNullOrWhiteSpace(configValue);
    }
}
