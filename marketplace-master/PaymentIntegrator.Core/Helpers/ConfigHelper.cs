using System;
using System.Configuration;
using System.Globalization;

namespace PaymentIntegrator.Core.Helpers
{
    /// <summary>
    /// Helper responsible for getting values from web.config or app.config.
    /// </summary>
    public static class ConfigHelper
    {
        private static string GetSettingInternal(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static T ConvertTypeInternal<T>(string value, IFormatProvider provider)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value);
            }

            return (T)Convert.ChangeType(value, typeof(T), provider ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets a value from a configuration key. If the key is not found or is empty, an exception is thrown.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <returns></returns>
        public static string GetSetting(string key)
        {
            var value = GetSettingInternal(key);

            if (!string.IsNullOrEmpty(value))
                return value;

            throw new Exception("App setting not found in web.config/app.config: " + key);
        }

        /// <summary>
        /// Gets a value from a configuration key. If the key is not found or is empty, it assumes the default value.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <param name="defaultValue">Default value of setting.</param>
        /// <returns></returns>
        public static string GetSetting(string key, string defaultValue)
        {
            var value = GetSettingInternal(key);

            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets a value from a configuration key, converting to an expected type. If the key is not found or is empty, an exception is thrown.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string key)
        {
            return GetSetting<T>(key, null);
        }

        /// <summary>
        /// Gets a value from a configuration key, converting to an expected type. If the key is not found or is empty, an exception is thrown.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string key, IFormatProvider provider)
        {
            return ConvertTypeInternal<T>(GetSetting(key), provider);
        }

        /// <summary>
        /// Gets a value from a configuration key, converting to an expected type. If the key is not found or is empty, it assumes the default value.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <param name="defaultValue">Default value of setting.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string key, T defaultValue)
        {
            return GetSetting<T>(key, defaultValue, null);
        }

        /// <summary>
        /// Gets a value from a configuration key, converting to an expected type. If the key is not found or is empty, it assumes the default value.
        /// </summary>
        /// <param name="key">Name of key.</param>
        /// <param name="defaultValue">Default value of setting.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string key, T defaultValue, IFormatProvider provider)
        {
            var value = GetSettingInternal(key);

            return !string.IsNullOrEmpty(value) ? ConvertTypeInternal<T>(value, provider) : defaultValue;
        }

        /// <summary>
        /// Gets a value from a connection string key. If the key is not found or is empty, an exception is thrown.
        /// </summary>
        /// <param name="name">Name of connection string.</param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            var value = ConfigurationManager.ConnectionStrings[name];

            if (value != null && !string.IsNullOrEmpty(value.ConnectionString))
            {
                return value.ConnectionString;
            }

            throw new Exception("Connection string not found in web.config/app.config: " + name);
        }
    }
}