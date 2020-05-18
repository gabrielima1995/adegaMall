using System;
using System.Diagnostics;

namespace ExpressiveAnnotations
{
    public static class ExpressiveHelper
    {
        public static bool IsNonNullableValueType(this Type type)
        {
            Debug.Assert(type != null);

            return !type.IsNullable() && type.IsValueType;
        }

        public static bool IsNullable(this Type type)
        {
            Debug.Assert(type != null);

            return type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}