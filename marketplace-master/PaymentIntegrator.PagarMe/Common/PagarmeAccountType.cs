using PaymentIntegrator.Core.Common;
using System;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeAccountType
    {
        public PagarmeAccountType(string accountType)
        {
            Type = accountType;
        }

        public string Type { get; }

        public static PagarmeAccountType From(AccountType type)
        {
            string accountType = "";
            switch (type)
            {
                case Core.Common.AccountType.ContaCorrente:
                    accountType = "conta_corrente";
                    break;
                case Core.Common.AccountType.Poupanca:
                    accountType = "conta_poupanca";
                    break;
                default:
                    throw new IndexOutOfRangeException($"Pagarme account type '{type}' is not recognized.");
            }
            
            return new PagarmeAccountType(accountType);
        }

    }
}
