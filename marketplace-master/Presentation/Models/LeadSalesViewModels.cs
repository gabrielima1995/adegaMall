using Application.Config;
using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Models
{
    public class PurchaseCreditsPackVM
    {
        public PaymentOptions Payment { get; set; }

        public CreditsPack Pack { get; set; }

        public string CreditsPackId { get; set; }

        public SelectList GetInstallmentsSelectList()
        {
            var p = Payment;

            if (p.InstallmentOptions == null)
                p.InstallmentOptions = new List<InstallmentOption>();

            var textFormat = CultureInfo.CurrentCulture.GetStringLocalized("INSTALLMENT_OPTION_AMOUNT");
            var currency = MaaSConfig.Current.DefaultCurrency;

            var items = p.InstallmentOptions.Select(opt =>
            {
                var itemText = string.Format(textFormat, opt.InstallmentsCount, currency, opt.Amount);
                return new KeyValuePair<int, string>(opt.InstallmentsCount, itemText);
            });

            var selectList = new SelectList(items, "Key", "Value", p.Installments);

            return selectList;
        }
    }

    public class PurchaseConfirmationVM
    {
        public int CreditsBefore { get; set; }
        public int CreditsAfter { get; set; }
        public int CreditsAdded { get; set; }

        public DateTime PurchaseTime { get; set; }
        public DateTime ExpirationDate { get; set; }

        public PurchaseConfirmationVM()
        {
        }

        public PurchaseConfirmationVM(CreditsPackTransaction transaction, Seller seller)
        {
            CreditsAdded = transaction.CreditsBundle.InitialCredits;
            CreditsAfter = seller.LeadSalesProfile.TotalAvailableCredits;
            CreditsBefore = seller.LeadSalesProfile.TotalAvailableCredits -
                            transaction.CreditsBundle.InitialCredits;
            PurchaseTime =
                seller.PreferredTimeZone.ConvertTimeFromUtc(transaction.CreditsBundle.CreatedAt);
            ExpirationDate = transaction.CreditsBundle.ExpirationDate;
        }
    }

    #region Stripe

    public class StripePurchaseCreditsPackRequest
    {
        [RequiredLocalized]
        public string credits_pack_id { get; set; }

        public string payment_method_id { get; set; }

        public string payment_intent_id { get; set; }
    }

    #endregion Stripe
}