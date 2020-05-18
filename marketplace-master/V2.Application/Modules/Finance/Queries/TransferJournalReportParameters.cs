using Domain.Modules.Finance;
using System;

namespace V2.Application.Modules.Finance.Queries
{
    public class TransferJournalReportParameters
    {
        public string SellerId { get; set; }

        public int StartIndex { get; private set; }

        public int Limit { get; private set; }

        public TransferJournalEntryType? Type { get; private set; }

        public bool ShowOnlyCancelled { get; private set; }

        public bool ShowOnlyCompleted { get; private set; }

        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public TransferJournalReportParameters(
            string sellerId,
            int startIndex, int limit,
            TransferJournalEntryType? type = null,
            bool showOnlyCancelled = false,
            bool showOnlyCompleted = false,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            SellerId = sellerId;
            StartIndex = startIndex;
            Limit = limit;
            Type = type;
            ShowOnlyCancelled = showOnlyCancelled;
            ShowOnlyCompleted = showOnlyCompleted;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
