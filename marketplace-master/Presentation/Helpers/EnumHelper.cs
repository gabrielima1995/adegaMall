using Infra.Crosscutting.Globalization;
using Infra.Crosscutting.Globalization.Attributes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public static class EnumHelper
    {
        public static SelectList SelectListFrom(Enum @enum)
        {
            var items = EnumOptionsToDictionary(@enum);
            return new SelectList(items, "Key", "Value", @enum);
        }

        public static string GetDescription(Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionLocalizedAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    var label = ((DescriptionLocalizedAttribute)attributes[0]).StringLabel;
                    return GlobalizationHelper.GetString(label);
                }
            }

            return value.ToString();
        }

        private static Dictionary<string, string> EnumOptionsToDictionary(Enum @enum)
        {
            var listItems = new Dictionary<string, string>();

            foreach (var value in Enum.GetValues(@enum.GetType()))
                listItems.Add(value.ToString(), GetDescription(value as Enum));

            return listItems;
        }
    }
}