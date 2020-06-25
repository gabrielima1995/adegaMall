using Dapper;
using Domain.Data.SqlScripts;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Domain.QuoteRequests;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class QuoteRepository : EfCrudRepository<Quote>, IQuoteRepository
    {
        public QuoteRepository(ScaffoldContext context) : base(context)
        {
        }

        public override Quote GetById(params object[] idValues)
        {
            var id = (Guid)idValues[0];

            return Context.Set<Quote>()
                .Include(x => x.Category)
                .Include(x => x.SubCategory)
                .Include(x => x.PortfolioItem)
                .Include(x => x.LatestAnswer)
                .Include(x => x.Answers)
                .Include(x => x.Attachments)
                .SingleOrDefault(x => x.Id == id);
        }

        public Quote GetDetails(Guid id)
        {
            var result = CacheHelper.Get(Context.Set<Quote>()
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Address)
                .Include(x => x.Category)
                .Include(x => x.SubCategory)
                .Include(x => x.PortfolioItem)
                .Include(x => x.Answers)
                .Include(x => x.Answers.Select(a => a.Attachments))
                .Include(x => x.Answers.Select(a => a.Buyer))
                .Include(x => x.Answers.Select(a => a.Seller))
                .Include(x => x.Attachments)
                .Where(x => x.Id == id));

            if (result != null)
                result.Answers = result.Answers.OrderBy(x => x.CreatedAt).ToList();

            return result;
        }

        public Quote GetDetailsForAdmin(Guid id)
        {
            return CacheHelper.Get(Context.Set<Quote>()
                .Include(x => x.Address)
                .Include(x => x.Buyer)
                .Include(x => x.Category)
                .Include(x => x.Seller)
                .Include(x => x.SubCategory)
                .Include(x => x.PortfolioItem)
                .Include(x => x.Answers)
                .Where(x => x.Id == id));
        }

        public List<Quote> GetListByGroup(Guid groupId)
        {
            return Context.Quotes.Where(x => x.GroupId == groupId).ToList();
        }

        public List<Quote> List(string buyerId, string sellerId, QuoteStatus? status)
        {
            Func<List<Quote>> query = () =>
            {
                using (var conn = MyDbConnection.Create())
                {
                    var sql = SqlScriptHelper.GetScript(ScriptName.QuoteRepository_List);
                    var grid = conn.QueryMultiple(sql, new { buyerId, sellerId, status });

                    var quotes = grid.Read<Quote>(
                        new[]
                        {
                            typeof(Quote),
                            typeof(Address),
                            typeof(Seller),
                            typeof(Buyer),
                            typeof(Category),
                            typeof(SubCategory),
                            typeof(PortfolioItem),
                            typeof(QuoteAnswer)
                        },
                        obj =>
                        {
                            var q = obj[0] as Quote;
                            q.Address = obj[1] as Address;
                            q.Seller = obj[2] as Seller;
                            q.Buyer = obj[3] as Buyer;
                            q.Category = obj[4] as Category;
                            q.SubCategory = obj[5] as SubCategory;
                            q.PortfolioItem = obj[6] as PortfolioItem;
                            q.LatestAnswer = obj[7] as QuoteAnswer;
                            return q;
                        }).ToList();

                    var attachments = grid.Read<QuoteAttachment>();
                    var quoteIdsWithSellerAnswer = grid.Read<Guid>();
                    var quoteAnswerAuthorNames = grid.Read<QuoteAnswerAuthorNameDto>();

                    quotes.ForEach(q => q.Attachments = attachments.Where(a => a.QuoteId == q.Id).ToList());
                    quotes.ForEach(q => q.HasAnswerFromSeller = quoteIdsWithSellerAnswer.Contains(q.Id));

                    // Map quote answer author names.
                    foreach (var quote in quotes)
                    {
                        if (quote.LatestAnswer == null)
                            continue;

                        var authorNames = quoteAnswerAuthorNames.Single(info => info.Id == quote.LatestAnswer.Id);

                        var buyer = Buyer.CreateAsDto();
                        buyer.FullName = buyer.AlternativeName = authorNames.BuyerName;

                        var seller = Seller.CreateAsDto();
                        seller.FullName = seller.AlternativeName = authorNames.SellerName;

                        quote.LatestAnswer.Buyer = buyer;
                        quote.LatestAnswer.Seller = seller;
                    }

                    return quotes;
                }
            };

            return CacheHelper.Get(query, null);
        }

        public List<Quote> ListByGroupIds(IEnumerable<Guid> groupIds)
        {
            var quotes = Context.Quotes
                .Include(x => x.Category)
                .Include(x => x.SubCategory)
                .Include(x => x.Buyer)
                .Where(x => x.GroupId.HasValue)
                .Where(x => groupIds.Contains(x.GroupId.Value)).ToList();
            quotes = quotes.GroupBy(x => x.GroupId).Select(y => y.First()).ToList();
            return quotes;
        }

        private class QuoteAnswerAuthorNameDto
        {
            public Guid Id { get; set; }
            public string BuyerName { get; set; }
            public string SellerName { get; set; }
        }
    }
}
