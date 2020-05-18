using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.PayPal.Buttons
{
    public class PayPalSubscriptionButtonResult : PayPalButtonResult
    {
        public PayPalSubscriptionButtonResult(IPayPalConfig config, string button, string itemName, decimal amount, string currencyCode, int duration, UnitOfDuration unitOfDuration, string custom,
            IDictionary<string, string> variables = null, bool autoPost = false, string messageAutoPost = "Processing, please wait...")
            : base(config, "_xclick-subscriptions", variables, autoPost, messageAutoPost)
        {
            var amountString = amount.ToString(CultureInfo.GetCultureInfo("en-us"));

            AddVariable("item_name", itemName);
            AddVariable("amount", amountString);
            AddVariable("currency_code", currencyCode.ToUpper());
            AddVariable("p3", duration.ToString());
            AddVariable("t3", unitOfDuration.ToString());
            AddVariable("a3", amountString);
            AddVariable("bn", button);
            AddVariable("custom", custom);
        }
    }
}