using Domain.Entities;
using Domain.Entities.StoreModule;
using PaymentIntegrator.Core.Common;

namespace Application.Sellers.Moip
{
    public static class MoipHelpers
    {
        public static void ToSeller(this SellerMoipViewModel model, Seller seller)
        {
            seller.AccountableName = model.Person.FullName;
            seller.AccountableDocumentNumber = model.Person.TaxDocumentNumber;

            seller.BankAccount = seller.BankAccount ?? new BankAccount();
            seller.BankAccount.Bank = model.BankAccount.Bank ?? default(Bank);
            seller.BankAccount.AccountType = model.BankAccount.Type ?? default(AccountType);
            seller.BankAccount.SetBranch(model.BankAccount.AgencyNumber, model.BankAccount.AgencyCheckNumber);
            seller.BankAccount.SetNumber(model.BankAccount.AccountNumber, model.BankAccount.AccountCheckNumber);
        }
    }
}