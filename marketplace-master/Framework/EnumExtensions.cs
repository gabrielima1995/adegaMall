using System.Linq;

namespace System
{
    public static class EnumExtensions
    {
        public static bool In(this Enum self, params Enum[] values)
        {
            if (values == null || values.Length == 0)
                return false;

            return values.Any(x => x.Equals(self));
        }

        public static bool NotIn(this Enum self, params Enum[] values)
        {
            return !In(self, values);
        }
    }
}
