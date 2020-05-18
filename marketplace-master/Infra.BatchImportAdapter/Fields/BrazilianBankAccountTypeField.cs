using BatchImport.Fields;
using PaymentIntegrator.Core.Common;
using System;
using System.Text.RegularExpressions;

namespace Infra.BatchImportAdapter.Fields
{
    public class BrazilianBankAccountTypeField : RegexField
    {
        private const string pattern = @"^(conta\s+)?((poupan[cç]a)|(corrente))$";

        public BrazilianBankAccountTypeField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Informe 'poupança' ou 'corrente'")
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
        }

        public override object Parse(string input)
        {
            if (input == null) return null;

            var match = Regex.Match(input);
            var typeName = match.Groups[2].Value;

            return "corrente".Equals(typeName, StringComparison.OrdinalIgnoreCase)
                ? AccountType.ContaCorrente : AccountType.Poupanca;
        }
    }

}
