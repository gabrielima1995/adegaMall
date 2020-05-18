namespace PaymentIntegrator.Moip.Common.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class BankAccountList
    {
        [JsonProperty("bankAccounts")]
        public List<BankAccount> BankAccounts { get; set; }
    }
}