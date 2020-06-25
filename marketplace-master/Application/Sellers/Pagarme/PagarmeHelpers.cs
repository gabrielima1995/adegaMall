using Domain.Entities;
using Domain.Entities.StoreModule;
using PaymentIntegrator.Core.Common;

namespace Application.Sellers.Pagarme
{
    public static class PagarmeHelpers
    {
        public static void ToSeller(this SellerPagarmeViewModel model, Seller seller)
        {
            seller.AccountableName = model.LegalName;
            seller.AccountableDocumentNumber = model.DocumentNumber;

            seller.BankAccount = seller.BankAccount ?? new BankAccount();
            seller.BankAccount.Bank = model.BankCode ?? default(Bank);
            seller.BankAccount.AccountType = model.Type ?? default(AccountType);
            seller.BankAccount.SetBranch(model.Agencia, model.AgenciaDv);
            seller.BankAccount.SetNumber(model.Conta, model.ContaDv);
        }
    }
}
