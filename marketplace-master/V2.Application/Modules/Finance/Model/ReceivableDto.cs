using System;

namespace V2.Application.Modules.Finance.Model
{
    public class ReceivableDto
    {
        public string Id { get; set; }

        public short InstallmentNumber { get; set; }

        public DateTime TransferDueDate { get; set; }

        public decimal Amount { get; set; }

        public bool IsAmountAvailableForTransfer { get; set; }

        public bool IsAdvanced { get; set; }
    }
}
