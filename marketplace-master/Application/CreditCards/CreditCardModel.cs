using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.CreditCards
{
    public class CreditCardModel : ICreditCard
    {
        [DisplayNameLocalized("CREDIT_CARD_FULLNAME_LABEL")]
        [StringLengthLocalized(50)]
        public string FullName { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_NUMBER_LABEL")]
        public string Number { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_MONTH_LABEL")]
        public int Month { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_YEAR_LABEL")]
        public int Year { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_CVV_LABEL")]
        [StringLengthLocalized(4)]
        public string CVV { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_DOCUMENT_NUMBER_LABEL")]
        public string DocumentNumber { get; set; }

        [DisplayNameLocalized("CREDIT_CARD_DATE_OF_BIRTH_LABEL")]
        public DateTime? DateOfBirth { get; set; }
    }
}