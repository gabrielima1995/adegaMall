using PaymentIntegrator.Core.Common;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeBankCode
    {
        public PagarmeBankCode(string bankCode)
        {
            BankCode = bankCode;
        }

        public string BankCode { get; }

        public static PagarmeBankCode From(int bankCode)
        {
            return new PagarmeBankCode(bankCode.ToString("000"));
        }

        public static PagarmeBankCode From(Bank bank)
        {
            return From((int)bank);
        }

        public override string ToString() => BankCode;
    }
}
