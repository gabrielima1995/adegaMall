using PaymentIntegrator.Core.Suppliers;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeBankAccount
    {
        public PagarmeBankCode BankCode { get; }
        public PagarmeBranchOrAccount Branch { get; }
        public PagarmeBranchOrAccount Account { get; }
        public PagarmeDocumentNumber DocumentNumber { get; }
        public PagarmeLegalName LegalName { get; }
        public PagarmeAccountType AccountType { get; set; }

        private PagarmeBankAccount(PagarmeBankCode bankCode, PagarmeBranchOrAccount branch, PagarmeBranchOrAccount account, PagarmeDocumentNumber documentNumber, PagarmeLegalName legalName, PagarmeAccountType accountType)
        {
            BankCode = bankCode;
            Branch = branch;
            Account = account;
            DocumentNumber = documentNumber;
            LegalName = legalName;
            AccountType = accountType;
        }

        public static PagarmeBankAccount From(ISupplier supplier, IAccountable accountable, IBankAccount bankAccount)
        {
            return new PagarmeBankAccount(
                PagarmeBankCode.From(bankAccount.Bank),
                PagarmeBranchOrAccount.From(bankAccount.Branch),
                PagarmeBranchOrAccount.From(bankAccount.Number),
                PagarmeDocumentNumber.From(supplier, accountable),
                PagarmeLegalName.From(supplier, accountable),
                PagarmeAccountType.From(bankAccount.AccountType)); 
        }

        public object AsJson() => new
        {
            bank_code = BankCode.ToString(),
            agencia = Branch.Main,
            agencia_dv = Branch.Digit,
            conta = Account.Main,
            type = AccountType.Type,
            conta_dv = Account.Digit,
            document_number = DocumentNumber.ToString(),
            legal_name = LegalName.ToString()
        };
    }
}
