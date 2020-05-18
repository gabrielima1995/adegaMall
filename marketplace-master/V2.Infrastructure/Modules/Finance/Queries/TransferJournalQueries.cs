using Dapper;
using Domain.Entities;
using System.Linq;
using V2.Application.Modules.Finance.Model;
using V2.Application.Modules.Finance.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.Finance.Queries
{
    public class TransferJournalQueries : ITransferJournalQueries
    {
        public TransferJournalReportResult GetReport(TransferJournalReportParameters parameters)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("TransferJournalQueries_Report");

                var results = conn.Query<TransferJournalReportResultItem, Buyer, Seller, TransferJournalReportResultItem>(sql,
                    (t, b, s) =>
                    {
                        t.Buyer = b;
                        t.Seller = s;
                        return t;
                    },
                    parameters).ToList();

                return new TransferJournalReportResult
                {
                    Parameters = parameters,
                    TotalResultsCount = results.Count,
                    Items = results
                };
            }
        }
    }
}
