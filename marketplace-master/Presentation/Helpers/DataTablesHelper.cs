using System;
using System.Text.RegularExpressions;

namespace Presentation.Helpers
{
    public static class DataTablesHelper
    {
        private static readonly Regex numbersOnly = new Regex(@"[^\d]", RegexOptions.Compiled);
        private const int maxStrLen = 30;

        public static string GetDataOrder(bool value)
        {
            return value ? "0" : "1";
        }
        public static string GetDataOrder(bool? value)
        {
            return GetDataOrder(value ?? false);
        }
        public static string GetDataOrder(decimal value)
        {
            return numbersOnly.Replace(value.ToString(), "");
        }
        public static string GetDataOrder(decimal? value)
        {
            return numbersOnly.Replace((value ?? decimal.MaxValue).ToString(), "");
        }
        public static string GetDataOrder(double value)
        {
            return numbersOnly.Replace(value.ToString(), "");
        }
        public static string GetDataOrder(double? value)
        {
            return numbersOnly.Replace((value ?? double.MaxValue).ToString(), "");
        }
        public static string GetDataOrder(int value)
        {
            return value.ToString();
        }
        public static string GetDataOrder(int? value)
        {
            return (value ?? int.MaxValue).ToString();
        }
        public static string GetDataOrder(long value)
        {
            return value.ToString();
        }
        public static string GetDataOrder(long? value)
        {
            return (value ?? long.MaxValue).ToString();
        }
        public static string GetDataOrder(short value)
        {
            return value.ToString();
        }
        public static string GetDataOrder(short? value)
        {
            return (value ?? short.MaxValue).ToString();
        }
        public static string GetDataOrder(DateTime value)
        {
            return value.Ticks.ToString();
        }
        public static string GetDataOrder(DateTime? value)
        {
            return (value?.Ticks ?? DateTime.MaxValue.Ticks).ToString();
        }
        public static string GetDataOrder(TimeSpan value)
        {
            return value.Ticks.ToString();
        }
        public static string GetDataOrder(TimeSpan? value)
        {
            return (value?.Ticks ?? TimeSpan.MaxValue.Ticks).ToString();
        }
        public static string GetDataOrder(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (value.Length > maxStrLen) value = value.Substring(0, maxStrLen);
            return value.ToString();
        }
        public static string GetDataOrder(Guid value)
        {
            return GetDataOrder(value.ToString());
        }
        public static string GetDataOrder(Guid? value)
        {
            if (value != null) return GetDataOrder(value.ToString());
            return GetDataOrder(0);
        }
        public static string GetDataOrder(Enum value)
        {
            if (value == null) return "0";
            var cast = Convert.ChangeType(value, value.GetTypeCode());
            return GetDataOrder(cast);
        }
        public static string GetDataOrder(object value)
        {
            if (value == null) return "0";
            return GetDataOrder(value.ToString());
        }
    }

    public static class Dt
    {
        public static string Order(bool value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(bool? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(decimal value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(decimal? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(double value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(double? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(int value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(int? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(long value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(long? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(short value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(short? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(DateTime value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(DateTime? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(TimeSpan value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(TimeSpan? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(string value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(Guid value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(Guid? value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(Enum value) => DataTablesHelper.GetDataOrder(value);
        public static string Order(object value) => DataTablesHelper.GetDataOrder(value);
    }
}