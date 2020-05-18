using System.Collections.Generic;
using System.ComponentModel;

namespace System
{
    public static class ObjectHelper
    {
        public static Dictionary<string, string> ToDictionary(this object value)
        {
            var dictionary = new Dictionary<string, string>();

            if (value != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(value))
                {
                    if (descriptor?.Name != null)
                    {
                        var propValue = descriptor.GetValue(value);

                        if (propValue != null)
                            dictionary.Add(descriptor.Name, $"{propValue}");
                    }
                }
            }

            return dictionary;
        }
    }
}