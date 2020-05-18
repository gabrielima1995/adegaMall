using System.Text.RegularExpressions;

namespace Framework
{
    public static class PhoneNumberHelper
    {
        public static bool IsValidPhoneNumber(string phoneInput)
        {
            var breakdown = new PhoneBreakdown(phoneInput);
            return breakdown.Success;
        }

        public static PhoneBreakdown Breakdown(string phoneInput)
        {
            return new PhoneBreakdown(phoneInput);
        }
    }

    public class PhoneBreakdown
    {
        private static readonly Regex _reOnlyNumbers = new Regex(@"[^\d]");

        public string Ddd { get; private set; }
        public string Number { get; private set; }
        public long ParsedNumber { get; private set; }
        public bool Success { get; set; }

        public PhoneBreakdown(string phoneInput)
        {
            if (phoneInput == null)
                return;

            var pattern = @"^0?(\d{2})?\D*(9?\D*\d{4})\D*(\d{4})$";

            if (!Regex.IsMatch(phoneInput, pattern))
                return;

            var match = Regex.Match(phoneInput, pattern);
            Ddd = match.Groups[1].Value;
            Number = OnlyNumbers(match.Groups[2].Value) + OnlyNumbers(match.Groups[3].Value);

            long parsedNumber;
            if (!long.TryParse(Number, out parsedNumber)) return;
            ParsedNumber = parsedNumber;

            Success = true;
        }

        private string OnlyNumbers(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return _reOnlyNumbers.Replace(input, "");
        }
    }
}
