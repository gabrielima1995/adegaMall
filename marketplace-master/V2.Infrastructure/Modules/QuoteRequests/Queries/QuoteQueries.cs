using Application.Config;
using Dapper;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Modules.Portfolio;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.QuoteRequests;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using V2.Application.Modules.QuoteRequests.Model;
using V2.Application.Modules.QuoteRequests.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.QuoteRequests.Queries
{
    public class QuoteQueries : IQuoteQueries
    {
        private static bool ShowSellersWithoutAnswer => !MaaSConfig.Current.Quote.GeneralQuote.Enabled || MaaSConfig.Current.Quote.GeneralQuote.ShowSellersWithoutAnswer;

        public QuoteDetailsForAdminDto GetDetailsForAdmin(Guid quoteId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteQueries_GetDetailsForAdmin");
                var query = conn.QueryMultiple(sql, new { quoteId });
                var dto = new QuoteDetailsForAdminDto();

                dto.Buyer = query.ReadSingle<Buyer>();
                dto.Seller = query.ReadSingleOrDefault<Seller>();
                dto.Address = query.ReadSingleOrDefault<Address>();
                dto.Quote = query.ReadSingle<Quote>();
                dto.Quote.Category = query.ReadSingleOrDefault<Category>();
                dto.Quote.SubCategory = query.ReadSingleOrDefault<SubCategory>();
                dto.Answers = query.Read<QuoteAnswer>();
                dto.Quote.PortfolioItem = query.ReadSingleOrDefault<PortfolioItem>();

                var attachments = query.Read<QuoteAttachment>();
                dto.Quote.Attachments = attachments.Where(x => x.QuoteAnswerId == null).ToList();
                foreach (var a in dto.Answers) a.Attachments = attachments.Where(x => x.QuoteAnswerId == a.Id).ToList();

                return dto;
            }
        }

        public QuoteGroupDetailsForBuyer GetQuoteGroupDetailsForBuyer(Guid groupId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteQueries_GetQuoteGroupDetailsForBuyer");
                var query = conn.QueryMultiple(sql, new { groupId });

                var quote = query.Read<Quote, Category, SubCategory, PortfolioItem, Address, Buyer, Seller, Quote>(
                    (qt, cat, sub, portfolioItem, addr, buyer, seller) =>
                    {
                        qt.Category = cat;
                        qt.SubCategory = sub;
                        qt.PortfolioItem = portfolioItem;
                        qt.Address = addr;
                        qt.Buyer = buyer;
                        qt.Seller = seller;
                        return qt;
                    }).SingleOrDefault();

                var interaction = query.Read<QuoteGroupInteractionForBuyer>();
                var latestProposals = query.Read<QuoteProposal>();
                var hasOpenQuotes = query.ReadSingle<bool>();

                foreach (var inter in interaction)
                {
                    var proposal = latestProposals.LastOrDefault(prop => prop.SellerId == inter.SellerId);
                    inter.InteractionPrice = proposal?.Price;
                    inter.InteractionDeadline = proposal?.DeliveryDeadline;
                }

                return new QuoteGroupDetailsForBuyer
                {
                    Quote = quote,
                    SellerInteractions = interaction.Where(s => ShowSellersWithoutAnswer || s.HasInteraction),
                    TotalSellersCount = interaction.Count(),
                    AllClosed = !hasOpenQuotes
                };
            }
        }

        public QuoteListForBuyer GetQuoteListForBuyer(string buyerId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteQueries_GetQuoteListForBuyer");
                var query = conn.QueryMultiple(sql, new { buyerId });

                var quotes = query.Read<Quote, Category, SubCategory, PortfolioItem, Address, Buyer, Seller, Quote>(
                    (qt, cat, sub, portfolioItem, addr, buy, sell) =>
                    {
                        qt.Category = cat;
                        qt.SubCategory = sub;
                        qt.PortfolioItem = portfolioItem;
                        qt.Address = addr;
                        qt.Buyer = buy;
                        qt.Seller = sell;
                        return qt;
                    });
                var closedGroups = query.Read<GroupIsClosedQueryObject>();
                var topSellers = query.Read<QuoteListItemForBuyerTopSellerEx>();
                var interactions = query.Read<QuoteListItemForBuyerInteractionEx>();
                var billings = query.Read<QuoteBillingEx>();

                var dto = new QuoteListForBuyer
                {
                    Items = quotes.Select(quote =>
                    {
                        var relatedSellers = topSellers
                            .Where(s => s.GroupId == quote.GroupId);

                        var visibleSellers = relatedSellers
                            .Where(s => ShowSellersWithoutAnswer || s.HasInteraction);

                        var latestInteraction = quote.GroupId == null
                            ? interactions.SingleOrDefault(i => i.QuoteId == quote.Id)
                            : interactions.SingleOrDefault(i => i.GroupId == quote.GroupId);

                        var billing = billings.FirstOrDefault(x =>
                            (quote.GroupId != null && x.GroupId == quote.GroupId) || x.QuoteId == quote.Id);

                        var item = new QuoteListItemForBuyer
                        {
                            Quote = quote,
                            Billing = billing,
                            TotalSellersCount = relatedSellers.Count(),
                            TopSellers = visibleSellers.Take(3),
                            LatestInteraction = latestInteraction,
                            AllClosed = quote.GroupId.HasValue ? closedGroups.Any(gr => gr.AllClosed && gr.GroupId == quote.GroupId) : false
                        };

                        return item;
                    })
                };

                return dto;
            }
        }

        public ReadOnlyCollection<QuoteListForAdminDto> ListForAdmin(int pageSize, int startIndex, out int totalCount)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteQueries_ListForAdmin");
                var grid = conn.QueryMultiple(sql, new { startIndex, pageSize });

                totalCount = grid.ReadSingle<int>();
                return grid.Read<QuoteListForAdminDto>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<QuoteWithoutSellerListDto> ListWithoutSeller(int pageSize, int startIndex, out int totalCount)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteQueries_ListWithoutSellers");
                var grid = conn.QueryMultiple(sql, new { startIndex, pageSize });

                totalCount = grid.ReadSingle<int>();
                return grid.Read<QuoteWithoutSellerListDto>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<SellerGeneralDataDto> ListSellersWhoReceivedQuoteByGroupId(Guid groupId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT Seller.Id, Seller.AlternativeName, Seller.FullName, Seller.Email
                    FROM Person Seller
                    INNER JOIN Quote ON Seller.Id = Quote.Seller_Id
                    WHERE Quote.GroupId = @groupId
                    ORDER BY Seller.AlternativeName, Seller.FullName, Seller.Email";
                return conn.Query<SellerGeneralDataDto>(sql, new { groupId }).ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<SellerGeneralDataDto> ListSellerToSendQuoteTo()
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                SELECT Seller.Id, Seller.AlternativeName, Seller.FullName, Seller.Email
                FROM Person Seller
                WHERE Seller.Discriminator = 'Seller' AND Seller.Enabled = 1
                ORDER BY Seller.AlternativeName, Seller.FullName, Seller.Email";

                return conn.Query<SellerGeneralDataDto>(sql).ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<Guid> ListQuoteIdsInGroup(Guid groupId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"SELECT Id FROM Quote WHERE GroupId = @groupId";
                return conn.Query<Guid>(sql, new { groupId }).ToList().AsReadOnly();
            }
        }
    }

    internal class QuoteListItemForBuyerTopSellerEx : QuoteListItemForBuyerTopSeller
    {
        public Guid QuoteId { get; set; }
        public Guid GroupId { get; set; }
        public bool HasInteraction { get; set; }
    }

    internal class QuoteListItemForBuyerInteractionEx : QuoteListItemForBuyerInteraction
    {
        public Guid QuoteId { get; set; }
        public Guid GroupId { get; set; }
    }

    internal class QuoteBillingEx : QuoteBilling
    {
        public Guid? GroupId { get; set; }
    }

    internal class GroupIsClosedQueryObject
    {
        public Guid GroupId { get; set; }
        public bool AllClosed { get; set; }
    }

    internal class QuoteProposal
    {
        public string SellerId { get; set; }
        public decimal Price { get; set; }
        public DateTime DeliveryDeadline { get; set; }
    }
}
