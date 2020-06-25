namespace Domain.Config.General.Integrator
{
    public class StripeConfig
    {
        public string PublishableKey { get; set; }

        public string SecretKey { get; set; }

        /// <summary>
        /// Three-letter ISO currency code (https://www.iso.org/iso-4217-currency-codes.html), in lowercase. Must be a supported currency.
        /// </summary>
        public string Currency { get; set; } = "eur";
    }
}