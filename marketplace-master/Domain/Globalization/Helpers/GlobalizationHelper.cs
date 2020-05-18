using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Infra.Crosscutting.Globalization
{
    public static class GlobalizationHelper
    {
        private static List<Str> _strings;

        public static IReadOnlyCollection<Str> Strings => _strings.AsReadOnly();

        public static string GetString(string label)
        {
            if (string.IsNullOrEmpty(label)) return label;
            return CultureInfo.CurrentCulture.GetStringLocalized(label);
        }

        public static string FormatStringWithLanguage(string label, string lang, params object[] args)
        {
            if (string.IsNullOrEmpty(label)) return label;

            try
            {
                var fmt = GetString(label, lang);
                return string.Format(fmt, args);
            }
            catch (FormatException)
            {
                return GetString(label, lang);
            }
        }

        public static string FormatString(string label, params object[] args)
        {
            if (string.IsNullOrEmpty(label)) return label;
            var fmt = GetString(label);
            return string.Format(fmt, args);
        }

        public static string GetString(string label, string lang)
        {
            return GetString(label, GlobalizationEnumHelper.ParseLang(lang));
        }

        public static string GetString(string label, Lang lang)
        {
            if (string.IsNullOrEmpty(label)) return label;

            var str = _strings?.FirstOrDefault(i =>
                i.Label == label &&
                i.Lang == lang);

            return str?.Value ?? str?.Label ?? label;
        }

        public static void LoadStrings(ICrudRepository<Str> repository)
        {
            _strings = repository.List();
        }

        public static void UpdateString(ICrudRepository<Str> repository, string label, string lang, string value)
        {
            var str = UpdateStringInMemoryInternal(label, lang, value);
            if (str == null) return;
            repository.Update(str, true);
        }

        public static void UpdateStringInMemory(string label, string lang, string value)
        {
            UpdateStringInMemoryInternal(label, lang, value);
        }

        private static Str UpdateStringInMemoryInternal(string label, string lang, string value)
        {
            if (value == null)
                return null;

            var str = _strings.FirstOrDefault(i =>
                i.Label == label &&
                i.Lang == GlobalizationEnumHelper.ParseLang(lang));

            str.Value = value;
            return str;
        }
    }
}