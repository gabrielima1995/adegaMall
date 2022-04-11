using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class CreditCard : ICreditCard
    {
        [DisplayNameLocalized("CREDIT_CARD_FULLNAME_LABEL")]
        [StringLengthLocalized(50)]
        [RequiredLocalized]
        public string FullName { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_NUMBER_LABEL")]
        [StringLengthLocalized(19)]
        [RequiredLocalized]
        public string Number { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_MONTH_LABEL")]
        [RequiredLocalized]
        public int Month { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_YEAR_LABEL")]
        [RequiredLocalized]
        public int Year { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_CVV_LABEL")]
        [StringLengthLocalized(4, MinimumLength = 3)]
        [RequiredLocalized]
        public string CVV { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_DOCUMENT_LABEL")]
        public string DocumentNumber { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_DATE_OF_BIRTH_LABEL")]
        public DateTime? DateOfBirth { get; set; }
    }

    public class InstallmentOption
    {
        public int InstallmentsCount { get; set; }

        public decimal BaseAmount { get; set; }

        public decimal InterestAmount { get; set; }

        public decimal Amount => BaseAmount + InterestAmount;
    }

    public class PaymentOptions
    {
        public IList<InstallmentOption> InstallmentOptions { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_LABEL")]
        public CreditCard CreditCard { get; set; }

        public PaymentMethod Method { get; set; }

        public string PostbackUrl { get; set; }

        public ICreditCardResult CreditCardResult { get; set; }

        public string SessionId { get; set; }

        [DisplayNameLocalized("INSTALLMENTS_LABEL")]
        public int Installments { get; set; }

        public PaymentOptions()
        {
            InstallmentOptions = new List<InstallmentOption>();
        }

        public bool HasPayment()
        {
            return Method.In(PaymentMethod.Boleto, PaymentMethod.CreditCard);
        }
    }
}
