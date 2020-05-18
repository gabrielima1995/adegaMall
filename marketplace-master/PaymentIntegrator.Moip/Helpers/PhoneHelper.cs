using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Helpers
{
    public class PhoneHelper
    {
        public static void ExtractNumbers(string fullNumber, out string areaCode, out string phoneNumber)
        {
            var areaCodeRegex = new Regex(@"\(?([1-9]{2})\)?\s?([0-9\-]*)$");
            var match = areaCodeRegex.Match(fullNumber);
            var tempAreaCode = match.Groups[1].Value;
            var number = match.Groups[2].Value;

            number = number.Replace("-", "").Replace(" ", "").Replace(".", "");
            areaCode = tempAreaCode;
            phoneNumber = number;
        }
    }
}