using Dapper;
using Domain.Data.SqlScripts;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Entities.SubscriptionModule;
using Domain.Interfaces.Repositories;
using System.Linq;

namespace Infrastructure.Data
{
    public class AdminReportRepository : IAdminReportRepository
    {
        public AdminDashboardReport GetDashboardReport(AdminDashboardReportRequest request)
        {
            var sql = SqlScriptHelper.GetScript(ScriptName.Select_AdminDashboardReport);

            using (var conn = MyDbConnection.Create())
            {
                using (var multi = conn.QueryMultiple(sql, request))
                {
                    var model = new AdminDashboardReport(request);

                    model.Sellers = multi.Read<Seller>().ToList();
                    model.Buyers = multi.Read<Buyer>().ToList();

                    if (request.ProductEnabled)
                        model.SuccessfulOrderMetrics = multi.ReadSingle<AdminDashboardReport.SuccessfulOrderMetricsDto>();

                    if (request.QuoteEnabled)
                    {
                        model.Quotes = multi.Read<Quote>().ToList();
                        model.ClosedQuotes = multi.Read<Quote>().ToList();
                    }

                    if (request.SubscriptionEnabled)
                    {
                        model.Subscriptions = multi.Read<Subscription>().ToList();

                        var subTrans = multi.Read<SubscriptionTransaction>().ToList();

                        foreach (var s in model.Subscriptions)
                        {
                            s.Transactions = subTrans
                                .Where(x => x.Subscription_SellerId == s.SellerId)
                                .ToList();
                        }
                    }

                    if (request.CreditsPacksEnabled)
                    {
                        var read = multi.Read();
                        model.CreditsTransactionsTotal = 0;

                        if (read.First().Total != null)
                            model.CreditsTransactionsTotal = (decimal)(read.First().Total);

                        model.CreditsPacksPurchased = (int)read.First().Count;
                    }

                    return model;
                }
            }
        }

        private class AggregatedOrderMetricsAux
        {
            public int TotalOrdersCount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal TotalStartupFee { get; set; }
        }
    }
}
