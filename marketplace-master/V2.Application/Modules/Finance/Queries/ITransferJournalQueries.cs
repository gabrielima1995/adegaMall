using V2.Application.Modules.Finance.Model;

namespace V2.Application.Modules.Finance.Queries
{
    public interface ITransferJournalQueries
    {
        TransferJournalReportResult GetReport(TransferJournalReportParameters parameters);
    }
}
