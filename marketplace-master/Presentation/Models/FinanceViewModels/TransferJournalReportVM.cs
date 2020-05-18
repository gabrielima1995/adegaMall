using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.Finance.Model;
using V2.Application.Modules.Finance.Queries;

namespace Presentation.Models.FinanceViewModels
{
    public class TransferJournalReportVM
    {
        public TransferJournalReportParameters Parameters { get; set; }

        public int TotalResultsCount { get; set; }

        public IEnumerable<TransferJournalReportItemVM> Items { get; set; }

        public TransferJournalReportVM() { }

        public TransferJournalReportVM(TransferJournalReportResult result)
        {
            Parameters = result.Parameters;
            TotalResultsCount = result.TotalResultsCount;
            Items = result.Items.Select(x => new TransferJournalReportItemVM(x));
        }
    }
}