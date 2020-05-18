using PaymentIntegrator.Core.Suppliers;
using System;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeLegalName
    {
        private const int maxLength = 30;

        private PagarmeLegalName(string legalName)
        {
            LegalName = legalName;
        }

        public string LegalName { get; }

        public static PagarmeLegalName From(string legalName, bool isCnpj)
        {
            if (isCnpj)
                legalName = legalName.Truncate(maxLength);

            else if (legalName.Length > maxLength)
            {
                var parts = legalName.Split(' ');
                legalName = $"{parts[0]} {parts[parts.Length - 1]}";
            }

            return new PagarmeLegalName(legalName);
        }

        public static PagarmeLegalName From(string legalName, string documentNumber)
        {
            return From(legalName, PagarmeDocumentNumber.From(documentNumber).IsCnpj);
        }

        public static PagarmeLegalName From(ISupplier supplier, IAccountable accountable)
        {
            return From(accountable.Name ?? supplier.Name, accountable.DocumentNumber ?? supplier.DocumentNumber);
        }

        public override string ToString() => LegalName;
    }
}
