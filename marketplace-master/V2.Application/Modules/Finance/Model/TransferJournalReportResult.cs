using System.Collections.Generic;
using V2.Application.Modules.Finance.Queries;

namespace V2.Application.Modules.Finance.Model
{
    public class TransferJournalReportResult
    {
        public string SellerId { get; set; }

        public TransferJournalReportParameters Parameters { get; set; }

        public int TotalResultsCount { get; set; }

        public IEnumerable<TransferJournalReportResultItem> Items { get; set; }
    }
}
