using System.Text.RegularExpressions;

namespace PaymentIntegrator.PagarMe.Common
{
    /// <summary>
    /// Helper para string.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Extrai números presentes em uma string
        /// </summary>
        /// <param name="value">Valor a ser verificado</param>
        /// <returns></returns>
        public static string ExtractDigits(this string value)
        {
            return Regex.Replace(value, "[^0-9]", string.Empty);
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}