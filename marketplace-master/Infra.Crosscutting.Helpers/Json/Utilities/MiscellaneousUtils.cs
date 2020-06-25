namespace Infra.Crosscutting.Helpers.Json.Utilities
{
    internal class MiscellaneousUtils
    {
        internal static string FormatValueForPrint(object value)
        {
            if (value == null)
            {
                return "{null}";
            }

            if (value is string)
            {
                return @"""" + value + @"""";
            }

            return value.ToString();
        }
    }
}
