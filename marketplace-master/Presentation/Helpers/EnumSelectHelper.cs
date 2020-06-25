using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public static class EnumSelectHelper
    {
        private static bool HasFlagsInternal(Type type)
        {
            var attribute = type.GetCustomAttribute<FlagsAttribute>(inherit: false);
            return attribute != null;
        }

        public static bool IsValidForEnumHelper(Type type)
        {
            var isValid = false;
            if (type != null)
            {
                // Type.IsEnum is false for Nullable<T> even if T is an enum.  Check underlying type (if any).
                // Do not support Enum type itself -- IsEnum property is false for that class.
                var checkedType = Nullable.GetUnderlyingType(type) ?? type;
                if (checkedType.IsEnum)
                {
                    isValid = !HasFlagsInternal(checkedType);
                }
            }

            return isValid;
        }

        public static bool IsValidForEnumHelper(ModelMetadata metadata)
        {
            return metadata != null && IsValidForEnumHelper(metadata.ModelType);
        }

        public static IList<SelectListItem> GetSelectList(Type type, Func<string, string, string> textFormatted = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!IsValidForEnumHelper(type))
            {
                throw new ArgumentException("Invalid parameter type,", nameof(type));
            }

            if (textFormatted == null)
            {
                textFormatted = (value, text) => text;
            }
           

            IList<SelectListItem> selectList = new List<SelectListItem>();

            // According to HTML5: "The first child option element of a select element with a required attribute and
            // without a multiple attribute, and whose size is "1", must have either an empty value attribute, or must
            // have no text content."  SelectExtensions.DropDownList[For]() methods often generate a matching
            // <select/>.  Empty value for Nullable<T>, empty text for round-tripping an unrecognized value, or option
            // label serves in some cases.  But otherwise, ignoring this does not cause problems in either IE or Chrome.
            var checkedType = Nullable.GetUnderlyingType(type) ?? type;
            if (checkedType != type)
            {
                // Underlying type was non-null so handle Nullable<T>; ensure returned list has a spot for null
                selectList.Add(new SelectListItem { Text = string.Empty, Value = string.Empty, });
            }

            // Populate the list
            const BindingFlags bindingFlags =
                BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static;

            foreach (var field in checkedType.GetFields(bindingFlags))
            {
                // fieldValue will be an numeric type (byte, ...)
                var fieldValue = field.GetRawConstantValue();
                var fieldValueStr = fieldValue.ToString();
                var text = field.GetDescriptionLocalized();

                selectList.Add(new SelectListItem { Text =  textFormatted.Invoke(fieldValueStr, text), Value = fieldValueStr });
            }

            return selectList;
        }

        public static IList<SelectListItem> GetSelectList(Type type, Enum value, Func<string, string, string> textFormatted = null)
        {
            var selectList = GetSelectList(type, textFormatted);

            var valueType = value?.GetType();
            if (valueType != null && valueType != type && valueType != Nullable.GetUnderlyingType(type))
            {
                throw new ArgumentException("Invalid value parameter", nameof(value));
            }

            if (value == null && selectList.Count != 0 && string.IsNullOrEmpty(selectList[0].Value))
            {
                // Type is Nullable<T>; use existing entry to round trip null value
                selectList[0].Selected = true;
            }
            else
            {
                // If null, use default for this (non-Nullable<T>) enum -- always has 0 integral value
                var valueString = value?.ToString("d") ?? "0";

                // Select the last matching item, imitating what at least IE and Chrome highlight when multiple
                // elements in a <select/> element have a selected attribute.
                var foundSelected = false;
                for (var i = selectList.Count - 1; !foundSelected && i >= 0; --i)
                {
                    var item = selectList[i];
                    item.Selected = (valueString == item.Value);
                    foundSelected |= item.Selected;
                }

                // Round trip the current value
                if (!foundSelected)
                {
                    if (selectList.Count != 0 && string.IsNullOrEmpty(selectList[0].Value))
                    {
                        // Type is Nullable<T>; use existing entry for round trip
                        selectList[0].Selected = true;
                        selectList[0].Value = valueString;
                    }
                    else
                    {
                        // Add new entry which does not display value to user
                        selectList.Insert(0,
                            new SelectListItem { Selected = true, Text = string.Empty, Value = valueString, });
                    }
                }
            }

            return selectList;
        }
    }
}