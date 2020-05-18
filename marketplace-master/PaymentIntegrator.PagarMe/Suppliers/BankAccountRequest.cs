using Framework;
using PagarMe;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.PagarMe.Common;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.PagarMe.Suppliers
{
    /// <summary>
    /// Representa dados bancários para salvar no Pagar.me.
    /// </summary>
    public class BankAccountRequest : BankAccount, IValidatable
    {
        public BankAccountRequest(PagarMeClient client, ISupplier supplier, IBankAccount bankAccount, IAccountable accountable)
            : base(client.GetService())
        {
            var branchSplit = bankAccount.Branch?.Split('-') ?? new[] { "" };
            var numberSplit = bankAccount.Number?.Split('-') ?? new[] { "" };
            var documentNumber = SanitizeDocumentNumber(accountable?.DocumentNumber ?? supplier.DocumentNumber);

            BankCode = ((int)bankAccount.Bank).ToString("000");
            Agencia = branchSplit[0];
            AgenciaDv = branchSplit.Length > 1 ? branchSplit[1] : null;
            Conta = numberSplit[0];
            ContaDv = numberSplit.Length > 1 ? numberSplit[1] : null;
            DocumentType = GetDocumentType(documentNumber);
            DocumentNumber = documentNumber;
            LegalName = SanitizeLegalName(accountable?.Name ?? supplier.Name);
        }

        private DocumentType GetDocumentType(string documentNumber)
        {
            var cnpjRegex = new Regex(@"^([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|(\d{14})$");
            return cnpjRegex.IsMatch(documentNumber) ? DocumentType.Cnpj : DocumentType.Cpf;
        }

        private string SanitizeDocumentNumber(string input)
        {
            if (input == null)
                return null;

            return input.Replace(".", "").Replace("-", "").Replace("/", "");
        }

        private string SanitizeLegalName(string legalName)
        {
            if (legalName.Length > 30)
            {
                if (DocumentType == DocumentType.Cnpj)
                    legalName = legalName.Substring(0, 30);

                else
                {
                    var nameSplit = legalName.Split(' ');
                    legalName = nameSplit[0];

                    if (nameSplit.Length > 1)
                    {
                        var lastNames = "";

                        for (var i = nameSplit.Length - 1; i > 0 && legalName.Length + lastNames.Length + nameSplit[i].Length < 30; i--)
                            lastNames = " " + nameSplit[i] + lastNames;

                        legalName += lastNames;
                    }
                }
            }

            return legalName;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Agencia))
                errors.Add(nameof(Agencia), nameof(Agencia) + " é obrigatório.");

            if (string.IsNullOrEmpty(Conta))
                errors.Add(nameof(Conta), nameof(Conta) + " é obrigatório.");

            if (string.IsNullOrEmpty(ContaDv))
                errors.Add(nameof(ContaDv), nameof(ContaDv) + " é obrigatório.");

            if (string.IsNullOrEmpty(DocumentNumber))
                errors.Add(nameof(DocumentNumber), nameof(DocumentNumber) + " é obrigatório.");

            if (string.IsNullOrEmpty(LegalName))
                errors.Add(nameof(LegalName), nameof(LegalName) + " é obrigatório.");

            return errors;
        }
    }
}