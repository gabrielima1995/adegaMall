using Domain.Modules.Finance;
using System;
using V2.Application.Modules.Finance.Model;

namespace Presentation.Models.FinanceViewModels
{
    public class TransferJournalReportItemVM
    {
        public Guid Id { get; set; }

        public TransferJournalEntryType Type { get; set; }

        public Guid OrderId { get; set; }

        public string BuyerId { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAvatar { get; set; }

        public string BuyerEmail { get; set; }

        public string SellerId { get; set; }

        public string SellerName { get; set; }

        public string SellerAvatar { get; set; }

        public string SellerEmail { get; set; }

        public string ShortOrderId => OrderId.ToString().Substring(0, 8).ToLower();

        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal CommissionPct { get; set; }

        public short InstallmentsCount { get; set; }

        public short InstallmentNumber { get; set; }

        public string InstallmentStr => $"{InstallmentNumber}/{InstallmentsCount}";

        public decimal InstallmentAmount { get; set; }

        public decimal InstallmentCommissionAmount { get; set; }

        public decimal TransferAmount { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool Completed => CompletedDate != null;

        public DateTime? CancelledDate { get; set; }

        public bool Cancelled => CancelledDate != null;

        public string AdditionalNotes { get; set; }

        public TransferJournalReportItemVM() { }

        public TransferJournalReportItemVM(TransferJournalReportResultItem item)
        {
            Id = item.Id;
            Type = item.Type;
            OrderId = item.OrderId;

            BuyerId = item.Buyer.Id;
            BuyerName = item.Buyer.Name;
            BuyerAvatar = item.Buyer.Avatar;

            SellerId = item.Seller.Id;
            SellerName = item.Seller.Name;
            SellerAvatar = item.Seller.Avatar;

            PaymentMethod = item.PaymentMethod;
            OrderDate = item.OrderDate;
            OrderTotal = item.OrderTotal;
            CommissionPct = item.CommissionPct;

            InstallmentsCount = item.InstallmentsCount;
            InstallmentNumber = item.InstallmentNumber;
            InstallmentAmount = item.InstallmentAmount;
            InstallmentCommissionAmount = item.InstallmentCommissionAmount;

            TransferAmount = item.TransferAmount;
            DueDate = item.DueDate;
            CompletedDate = item.CompletedDate;
            CancelledDate = item.CancelledDate;
            AdditionalNotes = item.AdditionalNotes;
        }
    }
}