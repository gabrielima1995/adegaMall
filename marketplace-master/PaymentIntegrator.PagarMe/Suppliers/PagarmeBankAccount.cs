using PagarMe;

namespace PaymentIntegrator.PagarMe.Suppliers
{
    public class PagarmeBankAccount
    {
        public string BankCode { get; set; }
        public string Agencia { get; set; }
        public string AgenciaDv { get; set; }
        public string Conta { get; set; }
        public string ContaDv { get; set; }
        public string Type { get; set; }
        public string DocumentNumber { get; set; }
        public string LegalName { get; set; }

        public PagarmeBankAccount(BankAccount o)
        {
            BankCode = o.BankCode;
            Agencia = o.Agencia;
            AgenciaDv = o.AgenciaDv;
            Conta = o.Conta;
            ContaDv = o.ContaDv;
            Type = o.Type == AccountType.Poupanca ? "conta_poupanca" : "conta_corrente";
            DocumentNumber = o.DocumentNumber;
            LegalName = o.LegalName;
        }
    }
}