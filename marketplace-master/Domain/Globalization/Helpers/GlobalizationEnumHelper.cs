using Infra.Crosscutting.Globalization;
using Infra.Crosscutting.Globalization.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace System.Globalization
{
    public static class GlobalizationEnumHelper
    {
        private static string GetDescription(this FieldInfo field)
        {
            var display = field.GetCustomAttribute<DescriptionAttribute>();
            var description = display?.Description;
            return !string.IsNullOrEmpty(description) ? description : field.Name;
        }

        private static FieldInfo GetField(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return name != null ? type.GetField(name) : null;
        }

        public static string GetStringLabel(this FieldInfo field)
        {
            if (field == null) return null;
            var attr = field.GetCustomAttribute<DescriptionLocalizedAttribute>();
            return attr?.StringLabel;
        }

        public static string GetStringLabel(this Enum value)
        {
            var field = value.GetField();
            return field?.GetStringLabel();
        }

        public static string GetDescriptionLocalized(this FieldInfo field, string lang)
        {
            var stringLabel = field.GetStringLabel();
            return !string.IsNullOrEmpty(stringLabel)
                ? GlobalizationHelper.GetString(stringLabel, lang)
                : field.GetDescription();
        }

        public static string GetDescriptionLocalized(this FieldInfo field)
        {
            return GetDescriptionLocalized(field, CultureInfo.CurrentCulture.GetCurrentLang());
        }

        public static string GetDescriptionLocalized(this Enum value, string lang)
        {
            var field = value.GetField();
            return field?.GetDescriptionLocalized(lang);
        }

        public static string GetDescriptionLocalized(this Enum value)
        {
            return GetDescriptionLocalized(value, CultureInfo.CurrentCulture.GetCurrentLang());
        }

        public static Lang ParseLang(string lang)
        {
            return EnumHelper.ParseEnumByDescription(lang, Lang.pt_BR);
        }
    }
}