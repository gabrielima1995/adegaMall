using Application.Config;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.Metrics
{
    public class MetricsQueries : IMetricsQueries
    {
        public MetricsQueriesResultVM RunQuery(DateTime? startDate, DateTime? endDate)
        {
            using (var conn = MyDbConnection.Create())
            {
                MetricsQueriesResultVM viewModel = new MetricsQueriesResultVM
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                var parameters = new
                {
                    productEnabled = MaaSConfig.Current.Product.Enabled,
                    quoteEnabled = MaaSConfig.Current.Quote.Enabled,
                    buyerDecision = MaaSConfig.Current.Quote.BuyerDecision.Enabled,
                    startDate,
                    endDate
                };

                var grid = conn.QueryMultiple(SqlScriptHelper.GetScript("MetricsQueries_GetAdminAdvancedMetrics"), parameters);

                IEnumerable<dynamic> currentResult;

                try
                {
                    if (parameters.quoteEnabled)
                    {
                        currentResult = grid.Read();
                        viewModel.TotalIndividualQuotes = currentResult.First().IndividualQuotes ?? 0;

                        currentResult = grid.Read();
                        viewModel.TotalGroupQuotes = currentResult.First().GroupQuotes ?? 0;

                        currentResult = grid.Read();
                        viewModel.CountAcceptedQuotes = currentResult.First().Quotes_Won ?? 0;

                        currentResult = grid.Read();
                        viewModel.CountRejectedQuotes = currentResult.First().Quotes_Lost ?? 0;

                        currentResult = grid.Read();
                        viewModel.AverageAcceptedQuoteValue = currentResult.First().AverageWon ?? 0;

                        currentResult = grid.Read();
                        foreach (var line in currentResult)
                        {
                            viewModel.ListTopServiceSellers.Add(new MetricsQueriesItemVM()
                            {
                                Name = line.AlternativeName ?? line.FullName,
                                Url = MaaSConfig.Current.Environment.AppUrl + $"admin/sellers/details/{line.Id}/",
                                Sales = line.QuotesCount ?? 0,
                                SalesTotal = line.QuotesTotal ?? 0,
                            });
                        }


                        currentResult = grid.Read();
                        foreach (var line in currentResult)
                        {
                            viewModel.ListTopQuoteCategories.Add(new MetricsQueriesItemVM()
                            {
                                Name = line.Name,
                                Url = "",
                                Sales = line.C_Count ?? 0,
                                SalesTotal = line.Total ?? 0,
                            });
                        }
                    }

                    if (parameters.productEnabled)
                    {
                        currentResult = grid.Read();
                        viewModel.AverageOrderValue = currentResult.First().AverageOrder ?? 0;

                        currentResult = grid.Read();
                        viewModel.CountAbandonedCarts = currentResult.First().AbandonedCarts ?? 0;

                        currentResult = grid.Read();
                        foreach (var line in currentResult)
                        {
                            viewModel.ListTopProductSellers.Add(new MetricsQueriesItemVM
                            {
                                Name = line.AlternativeName ?? line.FullName,
                                Url = MaaSConfig.Current.Environment.AppUrl + $"admin/sellers/details/{line.Id}/",
                                Sales = line.Sales ?? 0,
                                SalesTotal = line.TotalSales ?? 0,
                            });
                        }

                        currentResult = grid.Read();
                        foreach (var line in currentResult)
                        {
                            viewModel.ListTopProducts.Add(new MetricsQueriesItemVM()
                            {
                                Name = line.Title,
                                Url = MaaSConfig.Current.Environment.AppUrl + $"product/{line.FriendlyUrl}/",
                                Sales = (int)(line.C_Count ?? 0m),
                                SalesTotal = line.Total ?? 0,
                            });
                        }

                        currentResult = grid.Read();
                        foreach (var line in currentResult)
                        {
                            viewModel.ListTopProductCategories.Add(new MetricsQueriesItemVM()
                            {
                                Name = line.Category,
                                Sales = (int)(line.C_Count ?? 0m),
                                SalesTotal = line.Total ?? 0,
                                Url = "",
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return viewModel;
                }
                return viewModel;
            }
        }

        public MetricsQueriesSellerResultVM RunSellerQuery(DateTime? startDate, DateTime? endDate, Guid sellerId)
        {
            MetricsQueriesSellerResultVM viewModel = new MetricsQueriesSellerResultVM();

            decimal fixedFee = 0M;
            decimal percentFee = 0M;

            var parameters = new
            {
                buyerDecision = MaaSConfig.Current.Quote.BuyerDecision.Enabled,
                startDate,
                endDate,
                fixedFee,
                percentFee,
                sellerId = sellerId.ToString(),
            };

            using (var conn = MyDbConnection.Create())
            {
                var grid = conn.QueryMultiple(SqlScriptHelper.GetScript("MetricsQueries_GetSellerAdvancedMetrics"), parameters);

                IEnumerable<dynamic> read;

                read = grid.Read();
                if (read.Any())
                {
                    viewModel.QuotesCount = read.First().NumQuotes ?? 0;
                    viewModel.RejectedQuotes = read.First().ClosedQuotes ?? 0;
                }

                read = grid.Read();
                if (read.Any())
                {
                    viewModel.AcceptedQuotes = read.First().NumQuotes ?? 0;
                    viewModel.AcceptedQuotesTotal = read.First().Total ?? 0;
                }

                if (parameters.buyerDecision)
                {
                    read = grid.Read();
                    if (read.Any())
                    {
                        viewModel.ProposalsSent = read.First().Proposals ?? 0;
                    }
                }

                read = grid.Read();
                if (read.Any())
                {
                    viewModel.AmountInWarranty = read.First().WarrantyAmount ?? 0.00m;
                }

                read = grid.Read();
                if (read.Any())
                {
                    viewModel.OrdersCompleted = read.First().NumSales ?? 0;
                    viewModel.TotalOrderRevenue = read.First().TotalRevenue ?? 0.0M;
                    viewModel.TotalOrderLiquidRevenue = read.First().TotalLiquidRevenue ?? 0.0M;
                    viewModel.ProductsSold = (int)(read.First().ProductsSold ?? 0);
                }

                read = grid.Read();
                if (read.Any())
                {
                    var date = read.First().ExpirationDate;

                    DateTime? expDate = null;
                    if (date != null)
                        expDate = (DateTime)date;

                    viewModel.NextCreditsExpirationDate = expDate;
                    viewModel.NextExpirationCredits = (int)read.First().CurrentCredits;
                    viewModel.TotalCredits = read.Sum(x => (int)x.CurrentCredits);
                }

                read = grid.Read();
                if (read.Any())
                {
                    viewModel.UnlockedQuotes = (int)read.First().UnlockedQuotes;
                }

                read = grid.Read();
                if (read.Any())
                {
                    var date = read.First().NextCreditsAddDate;

                    DateTime? nextDate = null;
                    if (date != null)
                        nextDate = (DateTime)date;

                    viewModel.NextCreditsAddDate = nextDate;
                }
            }
            return viewModel;
        }

        public List<MetricsQueriesCartVM> RunCartsQuery(DateTime? startDate, DateTime? endDate)
        {
            var list = new List<MetricsQueriesCartVM>();

            using (var conn = MyDbConnection.Create())
            {
                var parameters = new { startDate, endDate };
                var result = conn.Query(SqlScriptHelper.GetScript("MetricsQueries_GetAbandonedCarts"), parameters);

                foreach (var line in result)
                {
                    list.Add(new MetricsQueriesCartVM()
                    {
                        CartId = line.Id.ToString(),
                        BuyerName = line.FullName,
                        Email = line.Email,
                        BuyerId = line.UserId.ToString(),
                        CartValue = line.Value ?? 0.00M,
                        CartDate = line.CreatedAt,
                    });
                }
            }

            return list;
        }
    }


}