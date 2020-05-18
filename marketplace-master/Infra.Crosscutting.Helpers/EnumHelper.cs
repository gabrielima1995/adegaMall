using System.ComponentModel;

namespace System
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);

                if (field != null)
                {
                    var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }

        public static T ParseEnumByDescription<T>(string description, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            if (string.IsNullOrEmpty(description)) return defaultValue;

            foreach (Enum item in Enum.GetValues(typeof(T)))
            {
                if (item.GetDescription().ToLower().Equals(description.Trim().ToLower())) return (T)(object)item;
            }

            return defaultValue;
        }
    }
}