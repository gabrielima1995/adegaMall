using System;

namespace Domain.DTO
{
    public class SellerScheduledTransferViewModel
    {
        public Guid Id { get; set; }
        public string OrderId { get; set; }
        public string BuyerId { get; set; }
        public int TransferAmount { get; set; } 
        public DateTime DueDate { get; set; }
        public bool Anticipated { get; set; }
        public bool TransferAuthorized { get; set; }
        public bool Transferred { get; set; }
        public bool Canceled { get; set; }
        public string GatewayPayableId { get; set; }
        public int Installments { get; set; }
        public int CurrentInstallment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BuyerFullName { get; set; }
    }
}
