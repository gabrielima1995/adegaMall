using PaymentIntegrator.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Application.Sellers.Pagarme
{
    public class SellerPagarmeViewModel : IEquatable<SellerPagarmeViewModel>
    {
        private string _documentNumber;

        public bool Enabled { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_LEGALNAME_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string LegalName { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_DOCUMENTNUMBER_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [DocumentNumberLocalized("PAGARME_SELLER_BANKACCOUNT_DOCUMENTNUMBER_ERROR", forceCheck: true)]
        public string DocumentNumber
        {
            get => _documentNumber;
            set => _documentNumber = value?.ExtractDigits();
        }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_BANKCODE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        public Bank? BankCode { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_TYPE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        public AccountType? Type { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_AGENCIA_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string Agencia { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_AGENCIADV_LABEL")]
        [StringLengthLocalized(4)]
        public string AgenciaDv { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_CONTA_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string Conta { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_BANKACCOUNT_CONTADV_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string ContaDv { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_TERMS1_LABEL")]
        [MustBeTrueIfLocalized("Enabled")]
        public bool Terms1 { get; set; }

        [DisplayNameLocalized("PAGARME_SELLER_TERMS2_LABEL")]
        [MustBeTrueIfLocalized("Enabled")]
        public bool Terms2 { get; set; }

        public static SellerPagarmeViewModel Enable()
        {
            return new SellerPagarmeViewModel
            {
                Enabled = true
            };
        }

        public static SellerPagarmeViewModel Load(PaymentIntegrator.PagarMe.Suppliers.PagarmeBankAccount model)
        {
            return new SellerPagarmeViewModel
            {
                Enabled = true,
                LegalName = model.LegalName,
                DocumentNumber = model.DocumentNumber,
                BankCode = (Bank)Enum.Parse(typeof(Bank), model.BankCode),
                Type = model.Type == "conta_poupanca" ? AccountType.Poupanca : AccountType.ContaCorrente,
                Agencia = model.Agencia,
                AgenciaDv = model.AgenciaDv,
                Conta = model.Conta,
                ContaDv = model.ContaDv,
                Terms1 = true,
                Terms2 = true
            };
        }

        public string GetDocumentNumber()
        {
            return DocumentNumber;
        }

        public bool Equals(SellerPagarmeViewModel other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public IEnumerable<object> GetEqualityComponents()
        {
            yield return LegalName;
            yield return DocumentNumber;
            yield return BankCode;
            yield return Type;
            yield return Agencia;
            yield return AgenciaDv;
            yield return Conta;
            yield return ContaDv;
        }

        public bool IsFilled()
        {
            return Enabled &&
                LegalName != null &&
                DocumentNumber != null &&
                BankCode != null &&
                Type != null &&
                Agencia != null &&
                Conta != null &&
                Terms1 && Terms2;
        }
    }
}