using PaymentIntegrator.Moip.Common.Models;

namespace PaymentIntegrator.Moip.Suppliers
{
    public class MoipAccountAndBankAccount
    {
        public Account Account { get; private set; }
        public BankAccount BankAccount { get; private set; }

        public MoipAccountAndBankAccount(Account account, BankAccount bankAccount)
        {
            Account = account;
            BankAccount = bankAccount;
        }
    }
}