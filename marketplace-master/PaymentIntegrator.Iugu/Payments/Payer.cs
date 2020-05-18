using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Iugu.Payments
{
    public class Payer
    {
        [JsonProperty("cpf_cnpj")]
        public string CpfCnpj { get; set; }

        public string Name { get; set; }

        [JsonProperty("phone_prefix")]
        public string PhonePrefix { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public PayerAddress Address { get; set; }
    }

    public class PayerAddress
    {
        public string Street { get; set; }

        public string Number { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        [JsonProperty("zip_code")]
        public string ZipCode { get; set; }

        public string Complement { get; set; }
    }

    public static class PhoneNumberHelper
    {
        public static void BreakPhoneNumber(string input, out string prefix, out string number)
        {
            prefix = null;
            number = null;

            if (string.IsNullOrEmpty(input))
                return;

            input = input.Trim();
            var hasCountryCode = input.StartsWith("+");

            input = Regex.Replace(input, @"\D", "");

            if (hasCountryCode && input.Length > 2)
                input = input.Substring(2);

            if (input.StartsWith("0"))
                input = input.Substring(1);

            if (input.Length >= 10)
            {
                prefix = input.Substring(0, 2);
                input = input.Substring(2);
            }

            number = input;
        }
    }
}
