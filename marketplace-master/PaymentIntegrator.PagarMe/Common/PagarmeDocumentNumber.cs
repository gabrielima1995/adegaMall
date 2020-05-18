using PaymentIntegrator.Core.Suppliers;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeDocumentNumber
    {
        private PagarmeDocumentNumber(string documentNumber, bool isCnpj)
        {
            DocumentNumber = documentNumber;
            IsCnpj = isCnpj;
        }

        public string DocumentNumber { get; }
        public bool IsCnpj { get; }
        public bool IsCpf => !IsCnpj;
        public string CnpjOrCpfTypeString => IsCnpj ? "cnpj" : "cpf";

        public static PagarmeDocumentNumber From(string documentNumber)
        {
            var onlyNumbers = Regex.Replace(documentNumber, @"\D", "");
            return new PagarmeDocumentNumber(onlyNumbers, onlyNumbers.Length > 11);
        }

        public static PagarmeDocumentNumber From(ISupplier supplier, IAccountable accountable)
        {
            return From(accountable.DocumentNumber ?? supplier.DocumentNumber);
        }

        public bool IsSameAs(PagarmeDocumentNumber another) => DocumentNumber == another.DocumentNumber;

        public override string ToString() => DocumentNumber;

        public static implicit operator PagarmeDocumentNumber(string documentNumber) => From(documentNumber);
    }
}
